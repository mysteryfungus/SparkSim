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
        if(controlsShown) return;

        foreach(var ConnectionButton in FindObjectsOfType<ConnectionButton>())
        {
            Debug.Log("Showing connection buttons");
            ConnectionButton.Show();
        }

        controlsShown = true;
    }

    public void ExitConnectionMode()
    {
        Debug.Log("Exiting connection mode");
        if(!controlsShown) return;

        foreach(var ConnectionButton in FindObjectsOfType<ConnectionButton>())
        {
            Debug.Log("Hiding connection buttons");
            ConnectionButton.Hide();
        }

        controlsShown = false;
    }

    public void Connect(Contact contact)
    {
        if(selectedContact == null)
        {
            Debug.Log("First contact of a connection selected");
            selectedContact = contact;
        }
        else
        {
            Debug.Log("Second contact of a connection selected");
            otherContact = contact;
        }

        if(selectedContact != null && otherContact != null)
        {
            Debug.Log("Adding connection between " + selectedContact.Guid + " and " + otherContact.Guid);
            connections.Add(new HashSet<Contact> { selectedContact, otherContact });
            onConnection.Invoke();

            GameObject wire = Instantiate(wirePrefab);
            wire.GetComponent<Wire>().SetStartPoint(selectedContact);
            wire.GetComponent<Wire>().SetEndPoint(otherContact);

            selectedContact = null;
            otherContact = null;
        }
    }

    public void RemoveConnection(HashSet<Contact> connection)
    {
        //Debug.Log("Removing connection between " + connection.First.Guid + " and " + connection.Last.Guid);
        connections.Remove(connection);
    }
}
