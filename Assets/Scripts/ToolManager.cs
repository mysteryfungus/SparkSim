using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] PlacementSystem system;

    // Перечисление доступных инструментов
    public enum Tools
    {
        Place,
        Connect,
        Drag,
        Remove
    }

    public Tools currentTool;

    void FixedUpdate()
    {
        HandleToolState();
    }

    // Обрабатывает состояние текущего инструмента
    private void HandleToolState()
    {
        switch (currentTool)
        {
            case Tools.Place:
                system.ToggleEditMode(true);
                system.ToggleDeleteMode(false);
                break;

            case Tools.Connect:
            case Tools.Drag:
                system.ToggleEditMode(false);
                system.ToggleDeleteMode(false);
                break;

            case Tools.Remove:
                system.ToggleEditMode(false);
                system.ToggleDeleteMode(true);
                break;
        }
    }
}
