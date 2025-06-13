using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ContextMenuUI : MonoBehaviour
{
    public static ContextMenuUI Instance { get; private set; }

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RectTransform menuPanel;

    private List<GameObject> currentButtons = new List<GameObject>();
    private ObjectContextMenu currentContextMenu;
    private bool suppressHideThisFrame = false;
    private GameObject overlay;
    private Vector3? menuWorldPosition = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Hide();
    }

    public void Show(List<ObjectContextMenu.MenuItem> items, Vector2 screenPosition, ObjectContextMenu contextMenu, Vector3? worldPosition = null)
    {
        Clear();
        currentContextMenu = contextMenu;
        menuPanel.gameObject.SetActive(true);
        menuWorldPosition = worldPosition;
        Vector2 pos = screenPosition;
        if (menuWorldPosition.HasValue)
            pos = Camera.main.WorldToScreenPoint(menuWorldPosition.Value);
        menuPanel.transform.DOScale(0, 0f);
        menuPanel.transform.DOScale(1, 0.125f);
        menuPanel.position = pos;
        suppressHideThisFrame = true;
        CreateOverlay();
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            var btnObj = Instantiate(buttonPrefab, menuPanel);
            btnObj.GetComponentInChildren<Text>().text = items[i].label;
            btnObj.GetComponent<Button>().onClick.AddListener(() => OnMenuItemClicked(index));
            currentButtons.Add(btnObj);
        }
    }

    private void CreateOverlay()
    {
        if (overlay != null) return;
        overlay = new GameObject("ContextMenuOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        var canvas = menuPanel.GetComponentInParent<Canvas>();
        overlay.transform.SetParent(canvas.transform, false);
        var rect = overlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var img = overlay.GetComponent<Image>();
        img.color = new Color(0,0,0,0); // полностью прозрачный
        overlay.GetComponent<Button>().onClick.AddListener(Hide);
        overlay.transform.SetAsFirstSibling(); // под меню
    }

    public void Hide()
    {
        menuPanel.transform.DOScale(0, 0.125f).OnComplete(() =>
        {
            menuPanel.gameObject.SetActive(false);
            Clear();
            currentContextMenu = null;
            menuWorldPosition = null;
            if (overlay != null)
            {
                Destroy(overlay);
                overlay = null;
            }
        });
    }

    private void Clear()
    {
        foreach (var btn in currentButtons)
            Destroy(btn);
        currentButtons.Clear();
    }

    private void OnMenuItemClicked(int index)
    {
        // Сохраняем ссылку на действие до Hide(), чтобы избежать удаления объекта до вызова действия
        var action = currentContextMenu?.GetMenuItems()[index].action;
        Hide();
        action?.Invoke();
    }

    void Update()
    {
        if (suppressHideThisFrame)
        {
            suppressHideThisFrame = false;
            return;
        }
        if (menuPanel.gameObject.activeSelf && menuWorldPosition.HasValue)
        {
            menuPanel.position = Camera.main.WorldToScreenPoint(menuWorldPosition.Value);
        }
    }
}
