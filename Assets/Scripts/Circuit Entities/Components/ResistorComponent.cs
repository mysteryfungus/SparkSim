using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
public class ResistorComponent : CircuitComponent {
    public double Resistance = 1000; // Ом
    public override DrawMode _drawMode {get; set;}
    
    public override void CreateSpiceModel(Circuit circuit) {
        var resistor = new Resistor("R1", 
            Contacts[0].NodeName, 
            Contacts[1].NodeName, 
            Resistance);
        circuit.Add(resistor);
    }
}