using System.Collections.Generic;
using UnityEngine;
using SpiceSharp;
using UnityEngine.Events;
using System;

public abstract class CircuitComponent : MonoBehaviour
{
    public GameObject model;
    public GameObject icon;
    public abstract void CreateSpiceModel(Circuit circuit);
    public List<Contact> Contacts { get; protected set; }
    public UnityEvent componentDeleted;
    public string id;
    public enum DrawMode
    {
        Model,
        Icon
    }
    public abstract DrawMode _drawMode { get; set; }

    public void SwitchDrawMode(DrawMode mode)
    {
        _drawMode = mode;
        model.SetActive(mode == DrawMode.Model);
        icon.SetActive(mode == DrawMode.Icon);
    }

    void Awake()
    {
        Type type = GetType();
        IDManager.RegisterType(type);
        gameObject.name = $"{type.FullName} {IDManager.AssignID(type)}";
        Contacts = new();
        Contacts.AddRange(GetComponentsInChildren<Contact>());
        id = type.FullName.ToUpper() + "-" + Guid.NewGuid().ToString();
    }
}