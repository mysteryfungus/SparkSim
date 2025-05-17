using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] PlacementSystem system;

    // Перечисление доступных инструментов
    public enum Tools
    {
        None,
        Place,
        Connect,
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
                system.ToggleEditMode(false);
                system.ToggleDeleteMode(false);
                break;
                
            case Tools.None:
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
