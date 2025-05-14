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

    [SerializeField] private GameObject contactHighlightPrefab; // Маркер для всех контактов
    [SerializeField] private GameObject selectedContactPrefab;  // Маркер для выбранного контакта
    private GameObject currentSelectedMarker; // Текущий маркер выбранного контакта
    private List<GameObject> activeContactMarkers = new(); // Список активных общих маркеров

    public Dictionary<Guid, Guid> ConnectedContacts = new Dictionary<Guid, Guid>();

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
            // Подсвечиваем все контакты при входе в режим (один раз)
            if (!isConnecting && activeContactMarkers.Count == 0)
            {
                HighlightAllContacts();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    Transform contact = FindNearestContact(hit.point);
                    if (contact != null)
                    {
                        if (pointA == null)
                        {
                            // Выбираем первый контакт
                            pointA = contact;
                            SelectContact(pointA); // Подсвечиваем выбранный контакт
                            isConnecting = true;
                        }
                        else if (contact != pointA)
                        {
                            // Выбираем второй контакт
                            pointB = contact;
                            CreateWire(pointA, pointB);
                            ResetConnectionState();
                        }
                    }
                }
            }

            // Отмена соединения
            if (Input.GetMouseButtonDown(1) && pointA != null)
            {
                ResetConnectionState();
            }
        }
        else
        {
            // Выход из режима соединения
            if (isConnecting || activeContactMarkers.Count > 0 || currentSelectedMarker != null)
            {
                ResetConnectionState();
            }
        }
    }

    private void ResetConnectionState()
    {
        DeselectContact();
        pointA = null;
        pointB = null;
        isConnecting = false;
        
        // Полностью обновляем маркеры
        ClearAllHighlights();
        if (toolManager.currentTool == ToolManager.Tools.Connect)
        {
            HighlightAllContacts();
        }
    }

    // Создает соединение между двумя точками
    private void CreateWire(Transform A, Transform B)
    {
        // Получаем компоненты Contact для обоих контактов
        Contact contactA = A.GetComponent<Contact>();
        Contact contactB = B.GetComponent<Contact>();

        if (contactA != null && contactB != null)
        {
            // Добавляем пару в словарь соединённых контактов
            if (!ConnectedContacts.ContainsKey(contactA.Guid) && !ConnectedContacts.ContainsKey(contactB.Guid))
            {
                ConnectedContacts[contactA.Guid] = contactB.Guid;
                ConnectedContacts[contactB.Guid] = contactA.Guid;
            }
        }

        // Создаём визуальное соединение (провод)
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
        // Получаем контакты, соединённые этим проводом
        Rope rope = wire.GetComponent<Rope>();
        if (rope != null && rope.StartPoint != null && rope.EndPoint != null)
        {
            Contact contactA = rope.StartPoint.GetComponent<Contact>();
            Contact contactB = rope.EndPoint.GetComponent<Contact>();

            if (contactA != null && contactB != null)
            {
                // Удаляем пару из словаря
                ConnectedContacts.Remove(contactA.Guid);
                ConnectedContacts.Remove(contactB.Guid);
            }
        }

        // Удаляем провод из трекинга
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

    private Transform FindNearestContact(Vector3 position, float radius = 0.5f)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, layerMask);
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (IsValidContact(hit.transform))
            {
                float distance = Vector3.Distance(position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hit.transform;
                }
            }
        }
        return nearest;
    }

    private bool IsValidContact(Transform contact)
    {
        return contact.CompareTag("Contact"); // Или другая проверка, например, по компоненту
    }

    // Подсвечивает все контакты на сцене
    private void HighlightAllContacts()
    {
        ClearAllHighlights(); // Очищаем старые маркеры

        Collider[] allContacts = Physics.OverlapSphere(Vector3.zero, 1000, layerMask); // Ищем все контакты
        foreach (var contact in allContacts)
        {
            if (IsValidContact(contact.transform))
            {
                GameObject marker = Instantiate(contactHighlightPrefab, contact.transform.position, Quaternion.identity);
                marker.transform.SetParent(contact.transform);
                activeContactMarkers.Add(marker);
            }
        }
    }

    // Очищает все общие маркеры контактов
    private void ClearAllHighlights()
    {
        for (int i = activeContactMarkers.Count - 1; i >= 0; i--)
        {
            if (activeContactMarkers[i] != null)
            {
                Destroy(activeContactMarkers[i]);
            }
        }
        activeContactMarkers.Clear();
    }

    private void SelectContact(Transform contact)
    {
        if (currentSelectedMarker != null)
        {
            Destroy(currentSelectedMarker);
        }

        // Находим общий маркер для этого контакта и заменяем его на выбранный
        for (int i = 0; i < activeContactMarkers.Count; i++)
        {
            if (activeContactMarkers[i] != null && 
                activeContactMarkers[i].transform.parent == contact)
            {
                Destroy(activeContactMarkers[i]);
                activeContactMarkers.RemoveAt(i);
                break;
            }
        }

        if (selectedContactPrefab != null)
        {
            currentSelectedMarker = Instantiate(selectedContactPrefab, contact.position, Quaternion.identity);
            currentSelectedMarker.transform.SetParent(contact);
        }
    }

    private void DeselectContact()
    {
        if (currentSelectedMarker != null)
        {
            Destroy(currentSelectedMarker);
            currentSelectedMarker = null;
        }
    }

    private void OnToolChanged(ToolManager.Tools newTool)
    {
        if (newTool != ToolManager.Tools.Connect)
        {
            ResetConnectionState();
        }
    }
}
