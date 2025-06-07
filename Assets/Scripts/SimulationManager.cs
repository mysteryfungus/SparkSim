using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;
using UnityEngine.Events;
using System;

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
        
        FormGroups();
        FetchComponents();

        status = SimulationStatus.Running;
        onSimulationRunning.Invoke();
    }

    public void StopSimulation()
    {
        if (status != SimulationStatus.Running) return;

        ClearGroups();
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

    private void FormGroups()
    {
        var groups = ConnectionManager.instance.GetConnectionGroups();
        foreach (var group in groups)
        {
            Guid groupGuid = Guid.NewGuid();
            foreach (var contact in group)
            {
                contact.SetTempName(groupGuid.ToString());
            }
        }
    }

    private void ClearGroups()
    {
        var groups = ConnectionManager.instance.GetConnectionGroups();
        foreach (var group in groups)
        {
            foreach (var contact in group)
            {
                contact.ResetTempName();
            }
        }
    }

    private void FetchComponents()
    {
        var components = FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None);
        foreach (var component in components)
        {
            component.CreateSpiceModel(circuit);
        }

        var entities = new List<SpiceSharp.Entities.IEntity>(circuit);
        Debug.Log($"Fetched entities for a circuit. Added entities: {string.Join(", ", entities)}");
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