using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;
using UnityEngine.Events;
using System.ComponentModel;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;

    public SimulationStatus status { get; private set; }
    public UnityEvent onSimulationRunning;
    public UnityEvent onSimulationEnded;
    private Circuit circuit;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        status = SimulationStatus.Idle;
        circuit = new Circuit();
    }

    public void StartSimulation()
    {
        if (status == SimulationStatus.Running) return;

        FetchComponents();

        status = SimulationStatus.Running;
        onSimulationRunning.Invoke();
    }

    public void StopSimulation()
    {
        if (status != SimulationStatus.Running) return;

        circuit.Clear();

        status = SimulationStatus.Idle;
        onSimulationEnded.Invoke();
    }

    public void ToggleSimulation()
    {
        switch (status)
        {
            case SimulationStatus.Idle:
                StartSimulation();
                break;

            case SimulationStatus.Running:
                StopSimulation();
                break;
        }
    }

    private void FetchComponents()
    {
        foreach (var component in FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None))
        {
            component.CreateSpiceModel(circuit);
        }

        var components = new List<SpiceSharp.Entities.IEntity>(circuit);
        Debug.Log($"Fetched components for a circuit. Added components: {string.Join(", ", components)}");
    }

    [System.Obsolete("This method is deprecated and probably won't work. Try making an actual simulation")]
    public void DummySimulation()
    {
        var ammeter = FindObjectOfType<AmmeterComponent>();
        ammeter.DummyOutput();
    }

    public enum SimulationStatus
    {
        Idle,
        Running,
        Failed
    }
}