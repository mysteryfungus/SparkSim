using System.Collections.Generic;
using UnityEngine;
using SpiceSharp;

public abstract class CircuitComponent : MonoBehaviour 
{
    public abstract void CreateSpiceModel(Circuit circuit);
    public List<Contact> Contacts { get; protected set; }
    public enum DrawMode{
        Model,
        Icon
    }
    public abstract DrawMode _drawMode {get; set;}
}