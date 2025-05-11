using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlaceableObject : MonoBehaviour
{
    public Vector2Int Size = Vector2Int.one;
    
    private bool isPlaced;
    private Material[] originalMaterials;
    private Renderer[] renderers;
    private Vector3 lastValidPosition;
    public bool isBeingDragged; // Флаг перемещения
    private Quaternion targetRotation;
    private Coroutine rotationCoroutine;
    [SerializeField] private float rotationDuration = 0.3f;

    public void Initialize()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }
        CorrectPivotPosition();
        lastValidPosition = transform.position;
    }

    private void CorrectPivotPosition()
    {
        Vector3 offset = new Vector3(
            (Size.x % 2 == 0) ? 0.5f : 0f,
            0f,
            (Size.y % 2 == 0) ? 0.5f : 0f
        );
        transform.position += offset;
    }

    public List<Vector2Int> GetOccupiedCells(Vector2Int gridPosition)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        int startX = gridPosition.x - Mathf.FloorToInt(Size.x / 2f);
        int startZ = gridPosition.y - Mathf.FloorToInt(Size.y / 2f);

        for (int x = 0; x < Size.x; x++)
        {
            for (int z = 0; z < Size.y; z++)
            {
                cells.Add(new Vector2Int(startX + x, startZ + z));
            }
        }
        return cells;
    }

    public void UpdateVisuals(bool isValid)
    {
        // Обновляем визуалы, если объект перемещается
        if (isPlaced && !isBeingDragged) return;

        Material targetMaterial = isValid ? 
            PlacementSystem.current.validMaterial : 
            PlacementSystem.current.invalidMaterial;

        foreach (Renderer r in renderers)
        {
            r.material = targetMaterial;
        }
    }

    public void OnPlacementComplete()
    {
        isPlaced = true;
        foreach (var r in renderers)
        {
            r.materials = originalMaterials;
        }
    }

    public void ResetToLastPosition()
    {
        transform.position = lastValidPosition;
    }

    public void Rotate()
    {
        if (rotationCoroutine != null) return;
        
        // Сохраняем исходное состояние
        Vector2Int originalSize = Size;
        Quaternion startRotation = transform.rotation;
        Vector3 originalPosition = transform.position;
        
        // Пробуем повернуть временно
        Size = new Vector2Int(originalSize.y, originalSize.x);
        transform.Rotate(Vector3.up, 90);
        CorrectPivotPosition();
        
        // Проверяем валидность
        Vector2Int gridPos = PlacementSystem.current.GetGridPosition(transform.position);
        bool isValid = PlacementSystem.current.IsPositionValid(gridPos, this);

        if (!isValid)
        {
            // Откатываем изменения
            Size = originalSize;
            transform.rotation = startRotation;
            transform.position = originalPosition;
            UpdateVisuals(false);
            return;
        }

        // Запускаем анимацию, если валидно
        targetRotation = transform.rotation;
        transform.rotation = startRotation;
        rotationCoroutine = StartCoroutine(AnimateRotation(startRotation, targetRotation));
    }

    private IEnumerator AnimateRotation(Quaternion from, Quaternion to)
    {
        float elapsed = 0;
        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(from, to, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        rotationCoroutine = null;
        
        // Обновляем занятые клетки
        PlacementSystem.current.FreeCells(this);
        PlacementSystem.current.OccupyCells(this, 
            PlacementSystem.current.GetGridPosition(transform.position));
    }

    public void ResetMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials;
        }
    }

    private List<Vector2Int> GetOccupiedCellsAfterRotation(Vector2Int gridPos, Vector2Int newSize)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        int startX = gridPos.x - Mathf.FloorToInt(newSize.x / 2f);
        int startZ = gridPos.y - Mathf.FloorToInt(newSize.y / 2f);

        for (int x = 0; x < newSize.x; x++)
        {
            for (int z = 0; z < newSize.y; z++)
            {
                cells.Add(new Vector2Int(startX + x, startZ + z));
            }
        }
        return cells;
    }

    public void OnBeginDrag()
    {
        isBeingDragged = true;
    }

    public void OnEndDrag()
    {
        isBeingDragged = false;
    }
}