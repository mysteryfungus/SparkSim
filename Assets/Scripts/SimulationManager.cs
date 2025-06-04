using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;
using System;
using UnityEngine.Events;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;

    public SimulationStatus status;
    public UnityEvent onSimulationRunning;
    public UnityEvent onSimulationEnded;

    void Awake()
    {
        instance = this;
        status = SimulationStatus.Idle;
    }

    public void StartSimulation()
    {
        status = SimulationStatus.Running;
        onSimulationRunning.Invoke();
    }

    public void StopSimulation()
    {
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

    [Obsolete]
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