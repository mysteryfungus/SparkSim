using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    private float mouseZCoord;
    private PlaceableObject placeableObject;
    private Vector3 targetPosition;
    private bool isDragging;
    private Vector3 originalPosition;

    [SerializeField] private float smoothTime = 0.05f; // Уменьшаем время сглаживания
    [SerializeField] private float maxSpeed = 20f; // Максимальная скорость перемещения
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

        originalPosition = transform.position;
        placeableObject.OnBeginDrag();
        PlacementSystem.current.StartMovingObject(placeableObject);

        // Улучшенное вычисление смещения
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            offset = transform.position - hitPoint;
        }
        else
        {
            offset = Vector3.zero;
        }
        
        isDragging = true;
    }

    private void Update()
    {
        if (isDragging)
        {
            // Улучшенное получение позиции мыши
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                targetPosition = PlacementSystem.current.SnapCoordinateToGrid(hitPoint + offset);
            }

            // Плавное движение с ограничением скорости
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime,
                maxSpeed
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
        Vector2Int gridPos = PlacementSystem.current.GetGridPosition(finalPosition);

        // Проверяем, находится ли позиция в пределах пегборда
        if (!PlacementSystem.current.IsWithinBoardBounds(gridPos))
        {
            // Если позиция вне пегборда, удаляем объект
            PlacementSystem.current.DeleteObject(gameObject);
            return;
        }

        // Проверяем валидность позиции
        bool isValid = PlacementSystem.current.IsPositionValid(gridPos, placeableObject);

        if (!isValid)
        {
            transform.position = originalPosition;
            placeableObject.ResetMaterials();
        }
        else
        {
            transform.position = finalPosition;
            placeableObject.ResetMaterials();
        }

        PlacementSystem.current.FinishMovingObject(placeableObject);
    }

    private void OnMouseOver()
    {
        if (!PlacementSystem.current.isEditMode) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            placeableObject.Rotate();
        }
    }
}