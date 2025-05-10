using UnityEngine;
using System.Collections.Generic;

public class PegboardManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    
    [Header("Visual Settings")]
    [SerializeField] private Material pegboardMaterial;
    [SerializeField] private float pegboardThickness = 0.1f;
    
    private Dictionary<Vector2Int, CircuitComponent> gridOccupancy = new Dictionary<Vector2Int, CircuitComponent>();
    private List<CircuitComponent> placedComponents = new List<CircuitComponent>();
    private GameObject pegboardCube;
    
    private void Start()
    {
        InitializeGrid();
    }
    
    private void InitializeGrid()
    {
        // Create pegboard cube
        pegboardCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pegboardCube.transform.SetParent(transform);
        
        // Set material
        Renderer renderer = pegboardCube.GetComponent<Renderer>();
        if (renderer != null && pegboardMaterial != null)
        {
            renderer.material = pegboardMaterial;
        }
        
        // Calculate dimensions
        float width = gridWidth * cellSize;
        float height = gridHeight * cellSize;
        
        // Position and scale the cube
        pegboardCube.transform.localPosition = new Vector3(width / 2f - cellSize / 2f, -pegboardThickness / 2f, height / 2f - cellSize / 2f);
        pegboardCube.transform.localScale = new Vector3(width, pegboardThickness, height);
        
        // Update box collider
        BoxCollider collider = pegboardCube.GetComponent<BoxCollider>();
        collider.size = Vector3.one; // Unity's primitive cube has unit size
        collider.center = Vector3.zero;
    }
    
    public bool CanPlaceComponent(CircuitComponent component, Vector2Int position, int rotation)
    {
        // Check if the component can be placed at the given position and rotation
        Vector2Int[] occupiedCells = component.GetOccupiedCells(position, rotation);
        
        foreach (Vector2Int cell in occupiedCells)
        {
            if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
                return false;
                
            if (gridOccupancy.ContainsKey(cell))
                return false;
        }
        
        return true;
    }
    
    public void PlaceComponent(CircuitComponent component, Vector2Int position, int rotation)
    {
        if (!CanPlaceComponent(component, position, rotation))
            return;
            
        Vector2Int[] occupiedCells = component.GetOccupiedCells(position, rotation);
        
        foreach (Vector2Int cell in occupiedCells)
        {
            gridOccupancy[cell] = component;
        }
        
        component.transform.position = new Vector3(position.x * cellSize, 0, position.y * cellSize);
        component.transform.rotation = Quaternion.Euler(0, rotation * 90, 0);
        placedComponents.Add(component);
    }
    
    public void RemoveComponent(CircuitComponent component)
    {
        if (!placedComponents.Contains(component))
            return;
            
        Vector2Int[] occupiedCells = component.GetOccupiedCells(
            new Vector2Int(Mathf.RoundToInt(component.transform.position.x / cellSize),
                          Mathf.RoundToInt(component.transform.position.z / cellSize)),
            Mathf.RoundToInt(component.transform.rotation.eulerAngles.y / 90));
            
        foreach (Vector2Int cell in occupiedCells)
        {
            gridOccupancy.Remove(cell);
        }
        
        placedComponents.Remove(component);
        Destroy(component.gameObject);
    }
    
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }
    
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * cellSize,
            0,
            gridPosition.y * cellSize
        );
    }
} 