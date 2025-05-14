using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System;

public class Contact : MonoBehaviour {
    public Guid Guid { get; private set; }
    public string NodeName { get; private set; }
    public CircuitComponent ParentComponent { get; set; }
    public string TemporaryNodeName { get; set; }

    void Awake() {
        ParentComponent = GetComponentInParent<CircuitComponent>();
        Guid = Guid.NewGuid();
        NodeName = "N_" + Guid.ToString("N");
        Debug.Log("Initialized contact " + NodeName);
    }
}