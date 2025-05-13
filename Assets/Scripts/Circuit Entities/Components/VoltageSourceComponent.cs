using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
public class VoltageSourceComponent : CircuitComponent {
    public double Voltage = 5; // Вольт
    
    public override void CreateSpiceModel(Circuit circuit) {
        var source = new VoltageSource("V1", 
            Contacts[0].NodeName, 
            Contacts[1].NodeName, 
            Voltage);
        circuit.Add(source);
    }
}