using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine.Events;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    private Contact selectedContact;
    private Contact otherContact;

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

    public List<HashSet<Contact>> ConnectionGroups
    {
        get
        {
            List<HashSet<Contact>> connectionGroupsList = new();
            foreach (HashSet<Contact> connection in connections)
            {
                if (connectionGroupsList.Count == 0)
                {
                    connectionGroupsList.Add(connection);
                    continue;
                }

                bool isOverlapping = false;
                foreach (HashSet<Contact> group in ConnectionGroups)
                {
                    if (group.Overlaps(connection))
                    {
                        group.UnionWith(connection);
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping)
                {
                    ConnectionGroups.Add(connection);
                }
            }
            return connectionGroupsList;
        }
    }
}
