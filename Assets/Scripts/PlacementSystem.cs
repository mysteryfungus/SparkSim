using UnityEngine;
using System.Collections.Generic;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem current;
    private Vector3 lastMousePosition;

    [Header("Grid Settings")]
    public GridLayout gridLayout;
    public LayerMask gridLayerMask;
    public Grid grid;

    [Header("Board Settings")]
    [SerializeField] public Vector2Int boardSize = new Vector2Int(10, 10); // Размер пегборда в клетках
    [SerializeField] public Vector2Int boardOffset = Vector2Int.zero; // Смещение начала координат пегборда

    [Header("Drag Settings")]
    public float lerpSpeed = 15f;

    [Header("Visual Settings")]
    public Material validMaterial;
    public Material invalidMaterial;

    [Header("System States")]
    public bool isEditMode;
    public bool isDeleteMode;

    // Словарь занятых клеток
    public Dictionary<Vector2Int, PlaceableObject> occupiedCells = new Dictionary<Vector2Int, PlaceableObject>();
    private PlaceableObject currentDraggingObject;

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    // Проверяет, находится ли позиция в пределах пегборда
    public bool IsWithinBoardBounds(Vector2Int position)
    {
        return position.x >= boardOffset.x && 
               position.x < boardOffset.x + boardSize.x &&
               position.y >= boardOffset.y && 
               position.y < boardOffset.y + boardSize.y;
    }

    // Проверяет, находятся ли все клетки объекта в пределах пегборда
    private bool AreAllCellsWithinBounds(List<Vector2Int> cells)
    {
        foreach (var cell in cells)
        {
            if (!IsWithinBoardBounds(cell))
                return false;
        }
        return true;
    }

    // Начинает процесс перетаскивания объекта
    public GameObject StartDragging(GameObject prefab)
    {
        if (currentDraggingObject != null) return null;

        // Создаем объект
        GameObject obj = Instantiate(prefab);
        currentDraggingObject = obj.GetComponent<PlaceableObject>();
        
        // Размещаем объект в центре пегборда
        Vector3 spawnPos = grid.CellToWorld(new Vector3Int(
            boardOffset.x + boardSize.x / 2,
            boardOffset.y + boardSize.y / 2,
            0
        ));
        
        // Инициализируем объект
        currentDraggingObject.Initialize();
        
        // Устанавливаем позицию после инициализации
        obj.transform.position = spawnPos;
        
        return obj;
    }

    // Обновляет позицию перетаскиваемого объекта
    public void UpdateDragging(GameObject draggedObject)
    {
        if (currentDraggingObject == null) return;

        Vector3 mousePos = GetGridMousePosition();
        Vector3 snappedPos = SnapCoordinateToGrid(mousePos);

        // Плавное движение объекта
        draggedObject.transform.position = Vector3.Lerp(
            draggedObject.transform.position,
            snappedPos,
            Time.deltaTime * lerpSpeed
        );

        // Принудительная фиксация при близости
        if (Vector3.Distance(draggedObject.transform.position, snappedPos) < 0.01f)
        {
            draggedObject.transform.position = snappedPos;
        }

        // Проверка валидности размещения
        bool isValid = CheckPlacementValidity();
        currentDraggingObject.UpdateVisuals(isValid);
    }

    // Завершает процесс перетаскивания объекта
    public void StopDragging(GameObject draggedObject)
    {
        Vector3 finalPos = SnapCoordinateToGrid(draggedObject.transform.position);
        Vector2Int gridPos = GetGridPosition(finalPos);

        // Проверяем, находится ли позиция в пределах пегборда
        if (!IsWithinBoardBounds(gridPos))
        {
            // Если позиция вне пегборда, удаляем объект
            Destroy(draggedObject);
            currentDraggingObject = null;
            return;
        }

        if (currentDraggingObject == null) return;

        if (CheckPlacementValidity())
        {
            OccupyCells(currentDraggingObject, gridPos);
            currentDraggingObject.OnPlacementComplete();
        }
        else
        {
            Destroy(draggedObject);
        }
        currentDraggingObject = null;
    }

    // Начинает процесс перемещения объекта
    public void StartMovingObject(PlaceableObject obj)
    {
        FreeCells(obj);
    }

    // Завершает процесс перемещения объекта
    public void FinishMovingObject(PlaceableObject obj)
    {
        Vector2Int gridPos = GetGridPosition(obj.transform.position);
        if (IsPositionValid(gridPos, obj))
        {
            OccupyCells(obj, gridPos);
        }
        else
        {
            // Позиция уже скорректирована в ObjectDrag
        }
    }

    // Проверяет валидность размещения объекта
    private bool CheckPlacementValidity()
    {
        Vector2Int gridPos = GetGridPosition(currentDraggingObject.transform.position);
        List<Vector2Int> cells = currentDraggingObject.GetOccupiedCells(gridPos);

        // Проверяем границы пегборда
        if (!AreAllCellsWithinBounds(cells))
            return false;

        // Проверяем занятость клеток
        foreach (var cell in cells)
        {
            if (occupiedCells.ContainsKey(cell)) return false;
        }
        return true;
    }

    // Проверяет, занята ли клетка
    public bool IsCellOccupied(Vector2Int cell)
    {
        return occupiedCells.ContainsKey(cell);
    }

    // Проверяет валидность позиции для объекта
    public bool IsPositionValid(Vector2Int gridPosition, PlaceableObject obj)
    {
        List<Vector2Int> cells = obj.GetOccupiedCells(gridPosition);

        // Проверяем границы пегборда
        if (!AreAllCellsWithinBounds(cells))
            return false;

        // Проверяем занятость клеток
        foreach (var cell in cells)
        {
            if (occupiedCells.ContainsKey(cell) && occupiedCells[cell] != obj)
                return false;
        }
        return true;
    }

    // Занимает клетки для объекта
    public void OccupyCells(PlaceableObject obj, Vector2Int gridPosition)
    {
        FreeCells(obj); // Сначала освобождаем старые клетки
        List<Vector2Int> cells = obj.GetOccupiedCells(gridPosition);
        foreach (var cell in cells)
        {
            occupiedCells[cell] = obj;
        }
    }

    // Привязывает координаты к сетке
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        return grid.GetCellCenterWorld(cellPosition);
    }

    // Получает позицию в сетке
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    // Получает позицию мыши в сетке
    private Vector3 GetGridMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, gridLayerMask))
        {
            return hit.point;
        }
        return SnapCoordinateToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // Освобождает клетки, занятые объектом
    public void FreeCells(PlaceableObject obj)
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (var pair in occupiedCells)
        {
            if (pair.Value == obj) keysToRemove.Add(pair.Key);
        }
        foreach (var key in keysToRemove)
        {
            occupiedCells.Remove(key);
        }
    }

    // Удаляет объект
    public void DeleteObject(GameObject obj)
    {
        PlaceableObject placeable = obj.GetComponent<PlaceableObject>();
        if (placeable != null)
        {
            FreeCells(placeable);
            Destroy(obj);

            // Обновляем список объектов
            if (currentDraggingObject == placeable)
            {
                currentDraggingObject = null;
            }
        }
    }

    private void Update()
    {
        if (currentDraggingObject != null)
        {
            // Постоянное обновление даже без движения мыши
            UpdateDragging(currentDraggingObject.gameObject);
        }
    }

    // Переключает режим редактирования
    public void ToggleEditMode(bool enabled) => isEditMode = enabled;

    // Переключает режим удаления
    public void ToggleDeleteMode(bool enabled) => isDeleteMode = enabled;
}