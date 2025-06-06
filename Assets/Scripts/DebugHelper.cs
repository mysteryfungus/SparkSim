using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [Button("List Node Names")]
    public void ListNodeNames()
    {
        Debug.Log(TextDecorator.Title("Node Names:"));
        ListFieldValues<Contact>("NodeName");
    }

    [Button("List Temporary Node Names")]
    public void ListTempNodeNames()
    {
        Debug.Log(TextDecorator.Title("Temporary Node Names:"));
        ListFieldValues<Contact>("TemporaryNodeName");
    }

    [Button("List Connection Groups")]
    public void ListConnectionGroups()
    {
        Debug.Log(TextDecorator.Title("Connection Groups:"));
        var groups = ConnectionManager.instance.GetConnectionGroups();

        if(!groups.Any())
        {
            Debug.Log(TextDecorator.ListPoint("NONE"));
            return;
        }

        for (int i = 0; i < groups.Count; i++)
        {
            Debug.Log(TextDecorator.SubTitle($"GROUP {i + 1}"));
            foreach (var contact in groups[i])
            {
                Debug.Log(TextDecorator.ListPoint(contact.NodeName));
            }
        }
    }
    
    private void ListFieldValues<T>(string fieldName) where T : UnityEngine.Object
    {
        var objects = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (var obj in objects)
        {
            var type = typeof(T);
            var field = type.GetField(fieldName);
            if (field != null)
            {
                var value = field.GetValue(obj);
                Debug.Log($"{obj.name}: {fieldName} = {value}");
            }
            else
            {
                var prop = type.GetProperty(fieldName);
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    Debug.Log($"{obj.name}: {fieldName} = {value}");
                }
                else
                {
                    Debug.LogWarning($"{type.Name} does not have a field or property named '{fieldName}'");
                }
            }
        }
    }
}

public static class TextDecorator
{
    public static string Title(string text) => Decorate(text, "===");
    public static string SubTitle(string text) => Decorate(text, "=");
    public static string ListPoint(string text) => " - " + text;

    private static string Decorate(string text, string decoration) => decoration + " " + text + " " + decoration;
}