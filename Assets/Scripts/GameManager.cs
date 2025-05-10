using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum ToolMode
    {
        Edit,
        Connect,
        Delete
    }
    
    [Header("References")]
    [SerializeField] private PegboardManager pegboardManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject wirePrefab;
    
    [Header("UI References")]
    [SerializeField] private Button editModeButton;
    [SerializeField] private Button connectModeButton;
    [SerializeField] private Button deleteModeButton;
    [SerializeField] private GameObject simulationUI;
    [SerializeField] private Button startSimulationButton;
    [SerializeField] private Button stopSimulationButton;
    
    [Header("Tool Button Colors")]
    [SerializeField] private Color selectedToolColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color normalToolColor = Color.white;
    
    private ToolMode currentToolMode = ToolMode.Edit;
    private CircuitComponent selectedComponent;
    private CircuitComponent startComponent;
    private Vector2Int startContact;
    private bool isSimulating = false;
    
    private void Start()
    {
        SetupUIButtons();
        UpdateToolModeUI();
    }
    
    private void SetupUIButtons()
    {
        if (editModeButton != null)
            editModeButton.onClick.AddListener(() => SetToolMode(ToolMode.Edit));
        if (connectModeButton != null)
            connectModeButton.onClick.AddListener(() => SetToolMode(ToolMode.Connect));
        if (deleteModeButton != null)
            deleteModeButton.onClick.AddListener(() => SetToolMode(ToolMode.Delete));
            
        if (startSimulationButton != null)
            startSimulationButton.onClick.AddListener(StartSimulation);
        if (stopSimulationButton != null)
            stopSimulationButton.onClick.AddListener(StopSimulation);
    }
    
    private void Update()
    {
        if (isSimulating)
            return;
            
        HandleToolActions();
    }
    
    private void HandleToolActions()
    {
        switch (currentToolMode)
        {
            case ToolMode.Edit:
                HandleEditMode();
                break;
            case ToolMode.Connect:
                HandleConnectMode();
                break;
            case ToolMode.Delete:
                HandleDeleteMode();
                break;
        }
    }
    
    private void HandleEditMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                CircuitComponent component = hit.collider.GetComponent<CircuitComponent>();
                if (component != null)
                {
                    selectedComponent = component;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedComponent = null;
        }
        
        if (selectedComponent != null)
        {
            // Handle component rotation
            if (Input.GetKeyDown(KeyCode.R))
            {
                int newRotation = Mathf.RoundToInt(selectedComponent.transform.rotation.eulerAngles.y / 90f) + 1;
                Vector2Int gridPos = pegboardManager.WorldToGridPosition(selectedComponent.transform.position);
                
                if (pegboardManager.CanPlaceComponent(selectedComponent, gridPos, newRotation))
                {
                    selectedComponent.transform.rotation = Quaternion.Euler(0, newRotation * 90, 0);
                }
            }
            
            // Handle component movement
            if (Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    Vector2Int gridPos = pegboardManager.WorldToGridPosition(hit.point);
                    int rotation = Mathf.RoundToInt(selectedComponent.transform.rotation.eulerAngles.y / 90f);
                    
                    if (pegboardManager.CanPlaceComponent(selectedComponent, gridPos, rotation))
                    {
                        selectedComponent.transform.position = pegboardManager.GridToWorldPosition(gridPos);
                    }
                }
            }
        }
    }
    
    private void HandleConnectMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                CircuitComponent component = hit.collider.GetComponent<CircuitComponent>();
                if (component != null)
                {
                    if (startComponent == null)
                    {
                        startComponent = component;
                        startContact = pegboardManager.WorldToGridPosition(hit.point);
                    }
                    else
                    {
                        Vector2Int endContact = pegboardManager.WorldToGridPosition(hit.point);
                        CreateWire(startComponent, component, startContact, endContact);
                        startComponent = null;
                    }
                }
            }
        }
    }
    
    private void HandleDeleteMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                CircuitComponent component = hit.collider.GetComponent<CircuitComponent>();
                if (component != null)
                {
                    pegboardManager.RemoveComponent(component);
                }
                
                Wire wire = hit.collider.GetComponent<Wire>();
                if (wire != null)
                {
                    wire.Disconnect();
                }
            }
        }
    }
    
    private void CreateWire(CircuitComponent start, CircuitComponent end, Vector2Int startPoint, Vector2Int endPoint)
    {
        GameObject wireObj = Instantiate(wirePrefab);
        Wire wire = wireObj.GetComponent<Wire>();
        wire.Connect(start, end, startPoint, endPoint);
    }
    
    private void SetToolMode(ToolMode mode)
    {
        currentToolMode = mode;
        UpdateToolModeUI();
    }
    
    private void UpdateToolModeUI()
    {
        // Update button colors
        if (editModeButton != null)
            editModeButton.image.color = currentToolMode == ToolMode.Edit ? selectedToolColor : normalToolColor;
        if (connectModeButton != null)
            connectModeButton.image.color = currentToolMode == ToolMode.Connect ? selectedToolColor : normalToolColor;
        if (deleteModeButton != null)
            deleteModeButton.image.color = currentToolMode == ToolMode.Delete ? selectedToolColor : normalToolColor;
    }
    
    public void StartSimulation()
    {
        isSimulating = true;
        simulationUI.SetActive(true);
        // TODO: Initialize SpiceSharp simulation
    }
    
    public void StopSimulation()
    {
        isSimulating = false;
        simulationUI.SetActive(false);
        // TODO: Clean up SpiceSharp simulation
    }
} 