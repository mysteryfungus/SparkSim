using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System.Collections.Generic;
using Unity.VisualScripting;

public class VoltageSourceComponent : CircuitComponent
{
    public double Voltage = 5; // Вольт
    public override DrawMode _drawMode { get; set; }


    public override void CreateSpiceModel(Circuit circuit)
    {
        if (circuit == null) { Debug.LogError("Circuit is null!"); return; }
        VoltageSource source = new VoltageSource(
            id,
            "0",
            Contacts[1].TemporaryNodeName,
            Voltage
        );
        circuit.Add(source);
    }
}