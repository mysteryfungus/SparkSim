using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class ToolboxManager : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;

    private short toggledButton;

    [SerializeField] private ToolManager toolManager;
    [SerializeField] private ComponentDrawer componentDrawer;

    public UnityEvent NoneSelected;
    public UnityEvent PlaceSelected;
    public UnityEvent ConnectSelected;
    public UnityEvent DeleteSelected;

    void Start()
    {
        toggledButton = (short)toolManager.currentTool;
        switch(toggledButton)
        {
            case 0:
                SelectNone();
                break;
            
            case 1:
                SelectPlace();
                break;
            
            case 2:
                SelectConnect();
                break;
            
            case 3:
                SelectRemove();
                break;
        }
    }

    public void SelectNone()
    {
        toolManager.currentTool = ToolManager.Tools.None;
        NoneSelected.Invoke();
        componentDrawer.SlideOut();
        ToggleButton();
    }

    public void SelectPlace()
    {
        toolManager.currentTool = ToolManager.Tools.Place;
        PlaceSelected.Invoke();
        componentDrawer.SlideIn();
        ToggleButton();
    }

    public void SelectConnect()
    {
        toolManager.currentTool = ToolManager.Tools.Connect;
        ConnectSelected.Invoke();
        componentDrawer.SlideOut();
        ToggleButton();
    }

    public void SelectRemove()
    {
        toolManager.currentTool = ToolManager.Tools.Remove;
        DeleteSelected.Invoke();
        componentDrawer.SlideOut();
        ToggleButton();
    }

    private void ToggleButton()
    {
        buttons[toggledButton].interactable = true;
        toggledButton = (short)toolManager.currentTool;
        buttons[toggledButton].interactable = false;
    }
}
