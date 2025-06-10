using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using SpiceSharp.Components;
using UnityEngine.Localization.SmartFormat.Utilities;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using System.CodeDom;
using Unity.VisualScripting;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;

    public SimulationStatus status { get; private set; }

    public UnityEvent onSimulationRunning;
    public UnityEvent onSimulationEnded;

    private Circuit circuit;

    public Dictionary<AmmeterComponent, RealCurrentExport> currentExports;
    public RealVoltageExport voltageExports;

    private HashSet<CircuitComponent> components;

    private RealtimeSimulation simulation;

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

        components = new();
        FormGroups();
        FetchComponents();

        List<IValueReader> valueReaders = new();
        foreach (var ammeter in FindObjectsByType<AmmeterComponent>(FindObjectsSortMode.None))
        {
            valueReaders.Add(ammeter);
        }

        simulation = new(circuit, valueReaders);
        simulation.StartSimulation();

        status = SimulationStatus.Running;
        onSimulationRunning.Invoke();
    }

    public void StopSimulation()
    {
        if (status != SimulationStatus.Running) return;

        simulation.StopSimulation();

        components.Clear();
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
        var _components = FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None);
        foreach (var component in _components)
        {
            components.Add(component);
            component.CreateSpiceModel(circuit);
        }

        var entities = new List<SpiceSharp.Entities.IEntity>(circuit);
        Debug.Log($"Fetched entities for a circuit. Added entities: {string.Join(", ", entities)}");
    }

    [Obsolete("This method is deprecated and probably won't work. Try making an actual simulation")]
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

    private class RealtimeSimulation
    {
        readonly Circuit circuit;
        readonly List<IValueReader> valueReaders;
        CancellationTokenSource cancellationToken;
        Task simulationTask;
        List<(AmmeterComponent, RealCurrentExport)> currentExports = new();
        List<RealVoltageExport> voltageExports;

        public RealtimeSimulation(Circuit circuit, List<IValueReader> valueReaders)
        {
            this.circuit = circuit; this.valueReaders = valueReaders;
            Reset();
        }

        public void Reset()
        {
            cancellationToken = null;
            simulationTask = null;
            currentExports.Clear();
        }

        public void StartSimulation()
        {
            Debug.Log("Starting the simulation...");
            cancellationToken = new();
            var token = cancellationToken.Token;

            Transient simulation = new("Simulation", 1e-3, double.PositiveInfinity);

            foreach (var reader in valueReaders)
            {
                switch (reader)
                {
                    case AmmeterComponent ammeter:
                        Debug.Log("Adding new ammeter to simulation readers");
                        var currentExport = new RealCurrentExport(simulation, ammeter.id);
                        currentExports.Add((ammeter, currentExport));
                        break;

                    default:
                        Debug.LogError("Reader type unknown, skipping");
                        break;
                }
            }

            Debug.Log("Simulation is ready. Proceeding to the task...");
            simulationTask = Task.Run(() =>
            {
                try
                {
                    Debug.Log("Task is awake. Starting...");
                    foreach (int _ in simulation.Run(circuit, Transient.ExportTransient))
                    {
                        Debug.Log("Simulation is running...");
                        Debug.Log($"Time in simulation: {simulation.Time}");
                        if (token.IsCancellationRequested)
                        {
                            Debug.Log("Simulation recieved cancellation request. Stopping...");
                            break;
                        }
                        foreach (var currentExport in currentExports)
                        {
                            Debug.Log($"Writing export data to {currentExport.Item1}");
                            currentExport.Item1.UpdateDisplay(currentExport.Item2.Value);
                        }
                    }
                    Debug.LogWarning("We are out of the loop. That's probably not good");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Simulation task crashed: {ex}");
                    StopSimulation();
                }
            }, token);
        }

        public void StopSimulation()
        {
            Debug.Log("Trying to stop the simulation...");
            cancellationToken?.Cancel();
            simulationTask?.Wait();
        }
    }
}