using System;
using System.Collections.Generic;

public static class IDManager
{
    private static readonly Dictionary<Type, uint> registry = new();

    public static void RegisterType(Type type)
    {
        registry.TryAdd(type, 0);
    }

    public static uint AssignID(Type type)
    {
        registry.TryGetValue(type, out uint id);
        id++;
        registry[type] = id;
        return id;
    }
}
