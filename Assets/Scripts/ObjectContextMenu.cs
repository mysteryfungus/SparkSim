using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContextMenu : MonoBehaviour
{
    public enum ComponentType
    {
        Resistor,
        Ammeter,
        VoltageSource
    }

    [SerializeField] private ComponentType componentType;

    [Serializable]
    public class MenuItem
    {
        public string label;
        public Action action;
        public MenuItem(string label, Action action)
        {
            this.label = label;
            this.action = action;
        }
    }

    private List<MenuItem> menuItems = new List<MenuItem>();

    void Start()
    {
        switch (componentType)
        {
            case ComponentType.Resistor:
                    AddMenuItem("Справка", () => Debug.Log("СПРАВКА"));
                    AddMenuItem("Повернуть", () => Debug.Log("ПОВЕРНУТЬ"));
                    AddMenuItem("Удалить", () => Debug.Log("УДАЛИТЬ"));
                break;

            case ComponentType.Ammeter:
                break;

            case ComponentType.VoltageSource:
                break;
        }
    }

    public void AddMenuItem(string label, Action action)
    {
        menuItems.Add(new MenuItem(label, action));
    }

    public List<MenuItem> GetMenuItems() => menuItems;

    // Вызывается ContextMenuHandler для отображения меню
    public void ShowMenu(Vector2 screenPosition, Vector3? worldPosition = null)
    {
        ContextMenuUI.Instance.Show(menuItems, screenPosition, this, worldPosition);
    }

    // Обработка нажатия на кнопку меню
    public void OnMenuItemClicked(int index)
    {
        if (index >= 0 && index < menuItems.Count)
            menuItems[index].action?.Invoke();
    }

    void OnMouseOver()
    {
        
    }
}
