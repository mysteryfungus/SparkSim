using System;
using UnityEngine;
using UnityEngine.Events;

public class PegboardCreator : MonoBehaviour
{
    // Событие, вызываемое при создании пегборда
    public UnityEvent PegboardCreate;

    // Размер пегборда
    [SerializeField] public Vector2Int size = new(1, 1);

    // Толщина пегборда
    private float _thickness = 0.01f;

    void Start()
    {
        Create(size);
    }

    // Создает пегборд заданного размера
    public void Create(Vector2Int size)
    {
        if (size.x > 0 && size.y > 0)
        {
            transform.localScale = new(size.x, _thickness, size.y);
            Debug.Log($"Created a pegboard with size {size}");
        }
        else
        {
            Debug.LogError("Can't create board with specified size");
            return;
        }

        // Вызываем событие после успешного создания
        PegboardCreate.Invoke();
    }

    // Очищает пегборд (пока не реализовано)
    public void ClearBoard()
    {
        Debug.LogError("Not yet implemented");
    }
}
