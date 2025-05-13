using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System;

public class Contact : MonoBehaviour {
    public string NodeName { get; private set; }
    public CircuitComponent ParentComponent { get; set; }

    void Awake() {
        ParentComponent = GetComponentInParent<CircuitComponent>();
        NodeName = "N_" + Guid.NewGuid().ToString("N");
        Debug.Log("Initialized contact " + NodeName);
    }
}