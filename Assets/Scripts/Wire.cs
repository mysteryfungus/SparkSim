using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    [SerializeField] private Material wireMaterial;
    [SerializeField] private float wireThickness = 0.1f;
    
    private LineRenderer lineRenderer;
    private CircuitComponent startComponent;
    private CircuitComponent endComponent;
    private Vector2Int startContact;
    private Vector2Int endContact;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer.material = wireMaterial;
        lineRenderer.startWidth = wireThickness;
        lineRenderer.endWidth = wireThickness;
        lineRenderer.positionCount = 2;
    }
    
    public void Connect(CircuitComponent start, CircuitComponent end, Vector2Int startPoint, Vector2Int endPoint)
    {
        startComponent = start;
        endComponent = end;
        startContact = startPoint;
        endContact = endPoint;
        
        startComponent.ConnectWire(this);
        endComponent.ConnectWire(this);
        
        UpdateWirePosition();
    }
    
    public void Disconnect()
    {
        if (startComponent != null)
            startComponent.DisconnectWire(this);
            
        if (endComponent != null)
            endComponent.DisconnectWire(this);
            
        Destroy(gameObject);
    }
    
    private void Update()
    {
        if (startComponent != null && endComponent != null)
        {
            UpdateWirePosition();
        }
    }
    
    private void UpdateWirePosition()
    {
        Vector3 startPos = startComponent.transform.position + new Vector3(startContact.x, 0, startContact.y);
        Vector3 endPos = endComponent.transform.position + new Vector3(endContact.x, 0, endContact.y);
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
    
    public CircuitComponent GetOtherComponent(CircuitComponent component)
    {
        if (component == startComponent)
            return endComponent;
        else if (component == endComponent)
            return startComponent;
            
        return null;
    }
    
    public Vector2Int GetContactPoint(CircuitComponent component)
    {
        if (component == startComponent)
            return startContact;
        else if (component == endComponent)
            return endContact;
            
        return Vector2Int.zero;
    }
} 