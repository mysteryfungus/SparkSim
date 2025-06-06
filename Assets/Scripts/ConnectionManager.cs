using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine.Events;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    private Contact selectedContact;
    private Contact otherContact;

    // Each connection is a pair of contacts
    private List<HashSet<Contact>> connections = new();

    private bool controlsShown = false;

    public UnityEvent onConnection;
    public GameObject wirePrefab;

    private void Awake()
    {
        Debug.Log("ConnectionManager Awake");
        instance = this;
    }

    public void EnterConnectionMode()
    {
        Debug.Log("Entering connection mode");
        if (controlsShown)
        {
            return;
        }

        foreach (var ConnectionButton in FindObjectsByType<ConnectionButton>(FindObjectsSortMode.None))
        {
            Debug.Log("Showing connection buttons");
            ConnectionButton.Show();
        }

        controlsShown = true;
    }

    public void ExitConnectionMode()
    {
        Debug.Log("Exiting connection mode");
        if (!controlsShown)
        {
            return;
        }

        foreach (var ConnectionButton in FindObjectsByType<ConnectionButton>(FindObjectsSortMode.None))
        {
            Debug.Log("Hiding connection buttons");
            ConnectionButton.Hide();
        }

        controlsShown = false;
    }

    public void Connect(Contact contact)
    {
        if (selectedContact == null)
        {
            Debug.Log("First contact of a connection selected");
            selectedContact = contact;
            return;
        }

        if (selectedContact == contact)
        {
            Debug.Log("Cannot connect a contact to itself");
            selectedContact = null;
            return;
        }

        Debug.Log($"Adding connection between {selectedContact.Guid} and {contact.Guid}");
        connections.Add(new HashSet<Contact> { selectedContact, contact });
        onConnection.Invoke();

        var wire = Instantiate(wirePrefab);
        var wireComponent = wire.GetComponent<Wire>();
        wireComponent.SetStartPoint(selectedContact);
        wireComponent.SetEndPoint(contact);

        selectedContact = null;
    }

    public void RemoveConnection(HashSet<Contact> connection)
    {
        connections.Remove(connection);
    }

    // Метод для получения групп соединённых контактов
    public List<HashSet<Contact>> GetConnectionGroups()
    {
        // Словарь для поиска группы по контакту
        Dictionary<Contact, HashSet<Contact>> contactToGroup = new();
        List<HashSet<Contact>> groups = new();

        foreach (var connection in connections)
        {
            HashSet<Contact> group = null;

            // Найти все группы, к которым уже принадлежат контакты из текущего соединения
            foreach (var contact in connection)
            {
                if (contactToGroup.TryGetValue(contact, out var existingGroup))
                {
                    if (group == null)
                    {
                        group = existingGroup;
                    }
                    else if (group != existingGroup)
                    {
                        // Объединить группы
                        group.UnionWith(existingGroup);
                        // Обновить ссылки в словаре
                        foreach (var c in existingGroup)
                        {
                            contactToGroup[c] = group;
                        }
                        groups.Remove(existingGroup);
                    }
                }
            }

            // Если ни один контакт не был в группе, создать новую
            if (group == null)
            {
                group = new HashSet<Contact>();
                groups.Add(group);
            }

            // Добавить все контакты соединения в группу и словарь
            foreach (var contact in connection)
            {
                group.Add(contact);
                contactToGroup[contact] = group;
            }
        }

        return groups;
    }
}
