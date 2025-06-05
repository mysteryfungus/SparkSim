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
        ParentComponent = GetComponentInParent<CircuitComponent>();
        Guid = Guid.NewGuid();
        NodeName = "N_" + Guid.ToString("N");
        ResetTempName();
        Debug.Log("Contact " + NodeName + " initialized");
    }

    public void SetTempName(string name)
    {
        TemporaryNodeName = name;
    }

    public void ResetTempName()
    {
        TemporaryNodeName = NodeName;
    }
}