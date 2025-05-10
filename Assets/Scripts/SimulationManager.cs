using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;

namespace SparkSim
{
    public class SimulationManager : MonoBehaviour
    {
        private static SimulationManager instance;
        public static SimulationManager Instance => instance;
        
        [Header("Simulation Settings")]
        [SerializeField] private float simulationTimeStep = 0.001f;
        [SerializeField] private float maxSimulationTime = 1f;
        
        private Circuit circuit;
        private Transient simulation;
        private Dictionary<CircuitComponent, string> componentNodes;
        private bool isSimulating = false;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
                
            componentNodes = new Dictionary<CircuitComponent, string>();
        }
        
        public void StartSimulation()
        {
            if (isSimulating)
                return;
                
            InitializeCircuit();
            isSimulating = true;
        }
        
        public void StopSimulation()
        {
            if (!isSimulating)
                return;
                
            isSimulating = false;
            circuit = null;
            simulation = null;
            componentNodes.Clear();
        }
        
        private void InitializeCircuit()
        {
            circuit = new Circuit();
            simulation = new Transient("Transient", simulationTimeStep, maxSimulationTime);
            
            // Add ground node
            circuit.Add(new VoltageSource("V0", "0", "gnd", 0));
            
            // Initialize all components
            CircuitComponent[] components = FindObjectsOfType<CircuitComponent>();
            foreach (var component in components)
            {
                component.OnSimulationStart();
            }
        }
        
        public string GetOrCreateNode(CircuitComponent component)
        {
            if (!componentNodes.ContainsKey(component))
            {
                string nodeName = $"node_{componentNodes.Count + 1}";
                componentNodes[component] = nodeName;
            }
            
            return componentNodes[component];
        }
        
        private void Update()
        {
            if (!isSimulating)
                return;
                
            // Update simulation
            simulation.Run(circuit);
            
            // Update all components
            CircuitComponent[] components = FindObjectsOfType<CircuitComponent>();
            foreach (var component in components)
            {
                component.OnSimulationStep(simulationTimeStep);
            }
        }
        
        private void OnDestroy()
        {
            if (isSimulating)
            {
                StopSimulation();
            }
        }
        
        public void AddComponent(IComponent component)
        {
            if (circuit != null)
            {
                circuit.Add(component);
            }
        }
        
        public void RemoveComponent(IComponent component)
        {
            if (circuit != null)
            {
                circuit.Remove(component);
            }
        }
    }
} 