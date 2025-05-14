using UnityEngine;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharp.Components;
using System;

public class SimulationManager : MonoBehaviour {
    private Circuit _circuit = new();
    private List<CircuitComponent> _components = new();
    private DC _dcSimulation;

    public bool isRunning = false;
    private bool isInitialized = false;
    private Dictionary<Guid, Guid> _connectedContacts;
    [SerializeField] private ConnectionSystem _connectionSystem;

    void Awake() {
        _connectedContacts = _connectionSystem.ConnectedContacts;
    }

    private void RegisterComponents()
    {
        _components.AddRange(FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None));
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
        UpdateNodeNamesForSimulation();
        isRunning = true;
    }

    public void StopSimulation() {
        isInitialized = false;
        isRunning = false;
        Flush();
        ResetNodeNames();
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

    public void UpdateNodeNamesForSimulation()
    {
        // Словарь для хранения групп соединённых контактов
        Dictionary<Guid, string> nodeGroups = new Dictionary<Guid, string>();

        foreach (var pair in _connectedContacts)
        {
            Guid contactA = pair.Key;
            Guid contactB = pair.Value;

            // Если оба контакта уже в группе, пропускаем
            if (nodeGroups.ContainsKey(contactA) && nodeGroups.ContainsKey(contactB))
                continue;

            // Если один из контактов уже в группе, добавляем второй в ту же группу
            if (nodeGroups.ContainsKey(contactA))
            {
                nodeGroups[contactB] = nodeGroups[contactA];
            }
            else if (nodeGroups.ContainsKey(contactB))
            {
                nodeGroups[contactA] = nodeGroups[contactB];
            }
            // Иначе создаём новую группу
            else
            {
                string newNodeName = $"Node_{Guid.NewGuid()}";
                nodeGroups[contactA] = newNodeName;
                nodeGroups[contactB] = newNodeName;
            }
        }

        // Применяем новые имена к контактам
        foreach (var contact in FindObjectsOfType<Contact>())
        {
            if (nodeGroups.TryGetValue(contact.Guid, out string newNodeName))
            {
                contact.TemporaryNodeName = newNodeName; // Используем временное поле
            }
        }
    }

    public void ResetNodeNames()
    {
        foreach (var contact in FindObjectsOfType<Contact>())
        {
            contact.TemporaryNodeName = null;
        }
    }
}