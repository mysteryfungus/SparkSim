using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;

namespace SparkSim.Components
{
    public class Resistor : CircuitComponent
    {
        [Header("Resistor Settings")]
        [SerializeField] private float resistance = 1000f; // Default 1k ohm
        [SerializeField] private float tolerance = 0.05f; // 5% tolerance
        
        private ResistorSimulation resistor;
        
        protected override void Start()
        {
            base.Start();
            
            // Set up the resistor's visual representation
            size = new Vector2Int(2, 1); // Resistor takes up 2x1 grid cells
            
            // Define contact points (left and right sides)
            contactPoints.Add(new Vector2Int(0, 0)); // Left contact
            contactPoints.Add(new Vector2Int(1, 0)); // Right contact
        }
        
        public override void OnSimulationStart()
        {
            base.OnSimulationStart();
            
            // Create SpiceSharp resistor component
            resistor = new ResistorSimulation("R1", resistance);
        }
        
        public override void OnSimulationStop()
        {
            base.OnSimulationStop();
            resistor = null;
        }
        
        public override void OnSimulationStep(float deltaTime)
        {
            base.OnSimulationStep(deltaTime);
            
            if (resistor != null)
            {
                // Update visual representation based on simulation results
                float current = resistor.GetCurrent();
                UpdateVisuals(current);
            }
        }
        
        private void UpdateVisuals(float current)
        {
            // Update material color based on current flow
            float intensity = Mathf.Clamp01(Mathf.Abs(current) / 0.1f); // Normalize current
            Color color = Color.Lerp(Color.gray, Color.red, intensity);
            
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (resistance <= 0)
            {
                resistance = 1000f;
                Debug.LogWarning("Resistance must be greater than 0. Setting to default value of 1kΩ.");
            }
            
            if (tolerance < 0 || tolerance > 1)
            {
                tolerance = 0.05f;
                Debug.LogWarning("Tolerance must be between 0 and 1. Setting to default value of 5%.");
            }
        }
    }

    // Helper class to manage SpiceSharp resistor simulation
    public class ResistorSimulation
    {
        private SpiceSharp.Components.Resistor spiceResistor;
        private DC dc;
        private string node1, node2;
        
        public ResistorSimulation(string name, float resistance)
        {
            // Get node names from SimulationManager
            node1 = SimulationManager.Instance.GetOrCreateNode(null); // We'll update these when connecting
            node2 = SimulationManager.Instance.GetOrCreateNode(null);
            
            // Create SpiceSharp resistor
            spiceResistor = new SpiceSharp.Components.Resistor(name);
            spiceResistor.Parameters.Resistance = resistance;
            
            // Create DC simulation
            dc = new DC("DC", "V1", -1, 1, 0.1);
            
            // Add resistor to circuit
            SimulationManager.Instance.AddComponent(spiceResistor);
        }
        
        public void UpdateNodes(string newNode1, string newNode2)
        {
            node1 = newNode1;
            node2 = newNode2;
            spiceResistor.Connect(node1, node2);
        }
        
        public float GetCurrent()
        {
            // Run simulation and get current
            // This is a simplified example - you'll need to properly set up the circuit
            return 0f; // Placeholder
        }
        
        public void Dispose()
        {
            if (spiceResistor != null)
            {
                SimulationManager.Instance.RemoveComponent(spiceResistor);
            }
        }
    }
} 