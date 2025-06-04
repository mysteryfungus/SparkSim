using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
public class VoltageSourceComponent : CircuitComponent {
    public double Voltage = 5; // Вольт
    public override DrawMode _drawMode {get; set;}

    public override void CreateSpiceModel(Circuit circuit) {
        var source = new VoltageSource(id, 
            Contacts[0].NodeName, 
            Contacts[1].NodeName, 
            Voltage);
        circuit.Add(source);
    }
}