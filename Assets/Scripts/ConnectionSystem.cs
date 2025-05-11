using GogoGaga.OptimizedRopesAndCables;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ConnectionSystem : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask wires;
    [SerializeField] private Material wireMaterial;

    private Transform pointA;
    private Transform pointB;
    private RaycastHit hit;

    [SerializeField] private ToolManager toolManager;

    private bool isConnecting = false; // Tracks if a connection process is ongoing
    private Dictionary<GameObject, List<GameObject>> objectToWires = new(); // Tracks wires connected to objects

    private void FixedUpdate()
    {
        HandleRemoveTool();
        HandleConnectTool();
    }

    // Обрабатывает инструмент удаления
    private void HandleRemoveTool()
    {
        if (toolManager.currentTool == ToolManager.Tools.Remove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100, wires))
                {
                    GameObject wire = hit.transform.gameObject;
                    RemoveWire(wire); // Remove the wire
                }
            }
        }
    }

    // Обрабатывает инструмент соединения
    private void HandleConnectTool()
    {
        if (toolManager.currentTool == ToolManager.Tools.Connect)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (pointA == null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        Debug.Log("Got A");
                        pointA = hit.transform;
                        isConnecting = true; // Start connection process
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (pointA != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        Debug.Log("Got B");
                        pointB = hit.transform;
                    }

                    if (pointB != null && pointA != pointB)
                    {
                        Debug.Log("We wirin!");
                        CreateWire(pointA, pointB);
                    }

                    // Reset points and visual feedback after connection
                    pointA = null;
                    pointB = null;
                    isConnecting = false;
                }
            }
        }
    }

    // Создает соединение между двумя точками
    private void CreateWire(Transform A, Transform B)
    {
        GameObject wire = new("Wire");
        wire.AddComponent<Rope>();
        wire.GetComponent<Rope>().SetStartPoint(A);
        wire.GetComponent<Rope>().SetEndPoint(B);
        wire.GetComponent<Rope>().ropeLength = 0.01f;
        wire.AddComponent<RopeMesh>();
        wire.GetComponent<RopeMesh>().ropeWidth = 0.05f;
        wire.GetComponent<RopeMesh>().material = wireMaterial;

        // Track the wire for both connected objects
        TrackWire(A.gameObject, wire);
        TrackWire(B.gameObject, wire);
    }

    // Tracks a wire for a given object
    private void TrackWire(GameObject obj, GameObject wire)
    {
        if (!objectToWires.ContainsKey(obj))
        {
            objectToWires[obj] = new List<GameObject>();
        }
        objectToWires[obj].Add(wire);

        // Add a listener to clean up wires if the object is destroyed
        var destroyable = obj.GetComponent<MonoBehaviour>()?.GetType().GetMethod("OnDestroyed");
        if (destroyable != null)
        {
            destroyable.Invoke(obj, new object[] { (Action)(() => RemoveAllWires(obj)) });
        }
    }

    // Removes a specific wire
    private void RemoveWire(GameObject wire)
    {
        foreach (var kvp in objectToWires)
        {
            if (kvp.Value.Contains(wire))
            {
                kvp.Value.Remove(wire);
            }
        }
        Destroy(wire);
    }

    // Removes all wires connected to a specific object
    private void RemoveAllWires(GameObject obj)
    {
        if (objectToWires.ContainsKey(obj))
        {
            foreach (var wire in objectToWires[obj])
            {
                Destroy(wire);
            }
            objectToWires.Remove(obj);
        }
    }
}
