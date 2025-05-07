using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    private float mouseZCoord;
    private PlaceableObject placeableObject;
    private Vector3 targetPosition;
    private bool isDragging;
    private Vector3 originalPosition; // Для хранения исходной позиции

    [SerializeField] private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        placeableObject = GetComponent<PlaceableObject>();
    }

    private void OnMouseDown()
    {
        if (PlacementSystem.current.isDeleteMode)
        {
            PlacementSystem.current.DeleteObject(gameObject);
            return;
        }

        if (!PlacementSystem.current.isEditMode) return;

        // Сохраняем позицию перед началом перемещения
        originalPosition = transform.position;
        placeableObject.OnBeginDrag();
        PlacementSystem.current.StartMovingObject(placeableObject);

        // Вычисляем смещение между объектом и мышью
        mouseZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void Update()
    {
        if (isDragging)
        {
            // Обновляем позицию объекта
            Vector3 newPosition = GetMouseWorldPos() + offset;
            targetPosition = PlacementSystem.current.SnapCoordinateToGrid(newPosition);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );

            // Проверяем валидность позиции
            bool isValid = PlacementSystem.current.IsPositionValid(
                PlacementSystem.current.GetGridPosition(targetPosition),
                placeableObject
            );
            placeableObject.UpdateVisuals(isValid);
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        placeableObject.OnEndDrag();

        // Проверяем финальную позицию
        Vector3 finalPosition = PlacementSystem.current.SnapCoordinateToGrid(targetPosition);
        bool isValid = PlacementSystem.current.IsPositionValid(
            PlacementSystem.current.GetGridPosition(finalPosition),
            placeableObject
        );

        if (!isValid)
        {
            // Возвращаем объект на исходную позицию
            transform.position = originalPosition;
            placeableObject.ResetMaterials();
        }
        else
        {
            // Фиксируем объект на новой позиции
            transform.position = finalPosition;
            placeableObject.ResetMaterials();
        }

        PlacementSystem.current.FinishMovingObject(placeableObject);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseOver()
    {
        if (!PlacementSystem.current.isEditMode) return;

        // Поворачиваем объект при нажатии клавиши R
        if (Input.GetKeyDown(KeyCode.R))
        {
            placeableObject.Rotate();
        }
    }
}