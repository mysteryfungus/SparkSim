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
using Cysharp.Threading.Tasks;

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

        foreach (var group in ConnectionManager.instance.GetConnectionGroups())
        {
            foreach (var contact in group)
            {
                Debug.Log($"Contact: {contact}, NodeName: {contact.NodeName}, TempName: {contact.TemporaryNodeName}");
            }
        }

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

        //simulation.StopSimulation();

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
            bool hasGroundNode = group.Any(contact => contact.NodeName == "0");
            if (hasGroundNode)
            {
                foreach (var contact in group)
                {
                    if (contact.NodeName != "0")
                    {
                        contact.StrictSetTempName("0");
                    }
                    Debug.Log($"New temp node: {contact.TemporaryNodeName}");
                }
            }
            else
            {
                Guid groupGuid = Guid.NewGuid();
                foreach (var contact in group)
                {
                    contact.SetTempName(groupGuid.ToString());
                    Debug.Log($"New temp node: {contact.TemporaryNodeName}");
                }
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
        var typeCount = new Dictionary<string, int>();
        Debug.Log("=== DIAGNOSTICS: Начало FetchComponents ===");
        foreach (var component in _components)
        {
            components.Add(component);
            component.CreateSpiceModel(circuit);
            string typeName = component.GetType().Name;
            if (!typeCount.ContainsKey(typeName)) typeCount[typeName] = 0;
            typeCount[typeName]++;
            Debug.Log($"Component: {typeName}, id: {component.id}, contacts: {component.Contacts.Count}");
            for (int i = 0; i < component.Contacts.Count; i++)
            {
                var contact = component.Contacts[i];
                Debug.Log($"  Contact[{i}]: NodeName={contact.NodeName}, TemporaryNodeName={contact.TemporaryNodeName}, Parent={contact.ParentComponent?.GetType().Name}");
            }
        }
        Debug.Log("=== DIAGNOSTICS: Количество компонентов по типам ===");
        foreach (var kv in typeCount)
            Debug.Log($"  {kv.Key}: {kv.Value}");

        var entities = new List<SpiceSharp.Entities.IEntity>(circuit);
        Debug.Log($"Fetched entities for a circuit. Added entities: {string.Join(", ", entities.Select(e => e.Name))}");
        Debug.Log($"=== DIAGNOSTICS: Всего сущностей в SpiceSharp Circuit: {entities.Count} ===");

        // Диагностика групп соединённых контактов
        var groups = ConnectionManager.instance.GetConnectionGroups();
        Debug.Log($"=== DIAGNOSTICS: Всего групп соединённых контактов: {groups.Count} ===");
        int groupIdx = 0;
        foreach (var group in groups)
        {
            Debug.Log($"  Группа {groupIdx}, контактов: {group.Count}");
            foreach (var contact in group)
            {
                Debug.Log($"    Contact: NodeName={contact.NodeName}, TemporaryNodeName={contact.TemporaryNodeName}, Parent={contact.ParentComponent?.GetType().Name}");
            }
            groupIdx++;
        }
        Debug.Log("=== DIAGNOSTICS: Конец FetchComponents ===");
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
        CancellationTokenSource cancellationToken = new();
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
            simulationTask = null;
            currentExports.Clear();
        }

        public void StartSimulation()
        {
            Debug.Log("Starting the simulation...");
            var token = cancellationToken.Token;

            Transient simulation = new("Simulation", 1e-3, 360000.0f);

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
            /*simulationTask = Task.Run(() =>
            {
                try
                {*/
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
                            Debug.Log($"Writing export data to {currentExport.Item1.ToSafeString()}");
                            Debug.Log($"The value in question is {currentExport.Item2.Value}, btw");
                            currentExport.Item1.UpdateDisplay(currentExport.Item2.Value);
                        }
                    }
                    Debug.LogWarning("We are out of the loop. That's probably not good");
                }
                /*catch (Exception ex)
                {
                    Debug.LogError($"Simulation task crashed: {ex}");
                    StopSimulation();
                }
            }, token);*/
        }

        /*public void StopSimulation()
        {
            Debug.Log("Trying to stop the simulation...");
            cancellationToken?.Cancel();
            simulationTask?.Wait();
        }*/
    //}
}