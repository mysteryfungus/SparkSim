using System;
using System.Collections.Generic;

public static class IDManager
{
    private static readonly Dictionary<Type, uint> registry = new()
    {
        {typeof(CircuitComponent), 0},
        {typeof(Contact), 0}
    };

    public static uint AssignID(Type type)
    {
        registry.TryGetValue(type, out uint id);
        id++;
        registry[type] = id;
        return id;
    }
}
