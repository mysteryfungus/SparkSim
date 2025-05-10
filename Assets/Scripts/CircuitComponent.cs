using UnityEngine;
using System.Collections.Generic;

public abstract class CircuitComponent : MonoBehaviour
{
    [Header("Component Settings")]
    [SerializeField] protected Vector2Int size = Vector2Int.one;
    [SerializeField] protected List<Vector2Int> contactPoints = new List<Vector2Int>();
    
    protected int currentRotation = 0;
    protected List<Wire> connectedWires = new List<Wire>();
    
    protected virtual void Start()
    {
        // Base implementation
    }
    
    protected virtual void OnValidate()
    {
        // Base implementation
    }
    
    public Vector2Int[] GetOccupiedCells(Vector2Int position, int rotation)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Vector2Int rotatedSize = rotation % 2 == 0 ? size : new Vector2Int(size.y, size.x);
        
        for (int x = 0; x < rotatedSize.x; x++)
        {
            for (int y = 0; y < rotatedSize.y; y++)
            {
                cells.Add(position + new Vector2Int(x, y));
            }
        }
        
        return cells.ToArray();
    }
    
    public Vector2Int[] GetContactPoints(Vector2Int position, int rotation)
    {
        List<Vector2Int> rotatedContacts = new List<Vector2Int>();
        
        foreach (Vector2Int contact in contactPoints)
        {
            Vector2Int rotatedContact = RotatePoint(contact, rotation);
            rotatedContacts.Add(position + rotatedContact);
        }
        
        return rotatedContacts.ToArray();
    }
    
    private Vector2Int RotatePoint(Vector2Int point, int rotation)
    {
        Vector2Int rotated = point;
        
        for (int i = 0; i < rotation; i++)
        {
            rotated = new Vector2Int(rotated.y, size.x - 1 - rotated.x);
        }
        
        return rotated;
    }
    
    public virtual void ConnectWire(Wire wire)
    {
        if (!connectedWires.Contains(wire))
        {
            connectedWires.Add(wire);
        }
    }
    
    public virtual void DisconnectWire(Wire wire)
    {
        connectedWires.Remove(wire);
    }
    
    public virtual void OnSimulationStart()
    {
        // Override in derived classes to handle simulation start
    }
    
    public virtual void OnSimulationStop()
    {
        // Override in derived classes to handle simulation stop
    }
    
    public virtual void OnSimulationStep(float deltaTime)
    {
        // Override in derived classes to handle simulation step
    }
    
    protected virtual void OnDestroy()
    {
        // Disconnect all wires when component is destroyed
        foreach (Wire wire in connectedWires)
        {
            if (wire != null)
            {
                wire.Disconnect();
            }
        }
    }
} 