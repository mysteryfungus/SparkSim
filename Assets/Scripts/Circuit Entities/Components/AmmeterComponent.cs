using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using TMPro;
using System.Collections.Generic;
using System;

public class AmmeterComponent : CircuitComponent, IValueReader
{

    [SerializeField]
    private TextMeshProUGUI _ammeterText;
    public override DrawMode _drawMode { get; set; }

    void Start()
    {
        UpdateDisplay(0);
    }

    public void UpdateDisplay(double value)
    {
        _ammeterText.text = $"Сила тока: {value} A";
    }

    public override void SwitchDrawMode(DrawMode mode)
    { 
        _drawMode = mode;
        model.SetActive(mode == DrawMode.Model);
        icon.SetActive(mode == DrawMode.Icon);
    }

    [Obsolete("This method is deprecated and probably won't work. Why are you even trying to use this?")]
    public void DummyOutput()
    {
        var voltage = FindObjectOfType<VoltageSourceComponent>();
        var resistance = FindObjectOfType<ResistorComponent>();
        var current = voltage.Voltage / resistance.Resistance;
        UpdateDisplay(current);
    }

    public override void CreateSpiceModel(Circuit circuit)
    {
        if (circuit == null) { Debug.LogError("Circuit is null!"); return; }
        Resistor ammeter = new Resistor(
            id,
            Contacts[0].TemporaryNodeName,
            Contacts[1].TemporaryNodeName,
            0
        );
        circuit.Add(ammeter);
    }

    public override string ToString()
    {
        return $"{{Name: {gameObject.name}, ID: {id}}}";
    }
}