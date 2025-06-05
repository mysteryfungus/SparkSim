using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using System.Collections.Generic;

public class ResistorComponent : CircuitComponent {
    public double Resistance = 1000; // Ом
    public override DrawMode _drawMode {get; set;}
    
    public override void CreateSpiceModel(Circuit circuit)
    {
        if (circuit == null) { Debug.LogError("Circuit is null!"); return; }
        var resistor = new Resistor(
            id,
            Contacts[0].NodeName,
            Contacts[1].NodeName,
            Resistance
        );
        circuit.Add(resistor);
    }
}