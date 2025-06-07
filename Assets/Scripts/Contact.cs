using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System;
using UnityEngine.Events;

public class Contact : MonoBehaviour
{
    public Guid Guid { get; private set; }
    public string NodeName { get; private set; }
    public CircuitComponent ParentComponent { get; set; }
    public string TemporaryNodeName { get; private set; }

    public UnityEvent contactDeleted;

    void Awake()
    {
        IDManager.RegisterType(typeof(Contact));
        gameObject.name = $"{GetType().FullName} {IDManager.AssignID(typeof(Contact))}";
        ParentComponent = GetComponentInParent<CircuitComponent>();
        Guid = Guid.NewGuid();
        NodeName = "NODE-" + Guid.ToString();
        ResetTempName();
        Debug.Log("Contact " + NodeName + " initialized");
    }

    public void SetTempName(string name)
    {
        TemporaryNodeName = "NODE-TEMP-" + name;
    }

    public void ResetTempName()
    {
        TemporaryNodeName = NodeName;
    }
}