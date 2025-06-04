using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using TMPro;
public class AmmeterComponent : CircuitComponent {

    [SerializeField]
    private TextMeshProUGUI _ammeterText;
    public override DrawMode _drawMode {get; set;}

    void Awake(){
        UpdateAmmeterText(0);
    }

    public void UpdateAmmeterText(double current){
        _ammeterText.text = $"Сила тока: {current} A";
    }

    [System.Obsolete]
    public void DummyOutput()
    {
        var voltage = FindObjectOfType<VoltageSourceComponent>();
        var resistance = FindObjectOfType<ResistorComponent>();
        var current = voltage.Voltage / resistance.Resistance;
        UpdateAmmeterText(current);
    }

    public override void CreateSpiceModel(Circuit circuit)
    {
        var ammeter = new VoltageSource(id,
            Contacts[0].NodeName,
            Contacts[1].NodeName,
            0
        );
        circuit.Add(ammeter);
    }
}