using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;

public class SimulationManager : MonoBehaviour {
    private Circuit _circuit = new();
    private List<CircuitComponent> _components = new();
    private DC _dcSimulation;

    public bool isRunning = false;
    private bool isInitialized = false;

    private void RegisterComponents()
    {
        _components = FindObjectsOfType<CircuitComponent>().ToList();
        foreach (var component in _components) {
            component.CreateSpiceModel(_circuit);
        }
    }

    private void FixedUpdate() {
        if (isRunning) {
            Simulate();
        }
    }

    public void StartSimulation() {
        if (!isInitialized) {
            RegisterComponents();
            isInitialized = true;
        }
        isRunning = true;
    }

    public void StopSimulation() {
        isInitialized = false;
        isRunning = false;
        Flush();
    }

    private void Simulate() {
        // Создаём DC анализ
        _dcSimulation = new DC("DC Analysis");
        
        // Создаём экспортеры
        var voltageExport = new RealVoltageExport(_dcSimulation, "out");
        var currentExport = new RealCurrentExport(_dcSimulation, "V1");
        
        // Запускаем симуляцию
        _dcSimulation.Run(_circuit);

        Debug.Log($"Voltage: {voltageExport.Value} V");
        Debug.Log($"Current: {currentExport.Value} A");
    }

    private void Flush() {
        _components.Clear();
        _circuit.Clear();
    }
}