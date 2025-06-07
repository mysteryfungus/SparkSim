using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System.Collections.Generic;

public class ResistorComponent : CircuitComponent
{
    public double Resistance = 1000; // Ом
    public override DrawMode _drawMode { get; set; }

    public override void CreateSpiceModel(Circuit circuit)
    {
        if (circuit == null) { Debug.LogError("Circuit is null!"); return; }
        Resistor resistor = new Resistor(
            id,
            Contacts[0].TemporaryNodeName,
            Contacts[1].TemporaryNodeName,
            Resistance
        );
        circuit.Add(resistor);
    }
}