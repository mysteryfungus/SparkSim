using UnityEngine;

public class ContextMenuHandler : MonoBehaviour
{
    private ObjectContextMenu contextMenu;
    private ToolManager toolManager;

    void Awake()
    {
        contextMenu = GetComponent<ObjectContextMenu>();
        toolManager = FindFirstObjectByType<ToolManager>();
    }

    void Update()
    {
        if (toolManager != null && toolManager.currentTool == ToolManager.Tools.None)
        {
            if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                // Проверяем, что курсор над этим объектом
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        // Открыть меню
                        contextMenu?.ShowMenu(Input.mousePosition, hit.point);
                    }
                }
            }
        }
    }
}
