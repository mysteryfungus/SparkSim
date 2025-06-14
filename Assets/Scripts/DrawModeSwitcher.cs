using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class DrawModeSwitcher : MonoBehaviour
{
    public static DrawModeSwitcher instance;
    private Text buttonText;
    public CircuitComponent.DrawMode drawMode = CircuitComponent.DrawMode.Model;
    public UnityEvent onDrawModeSwitch;

    void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        instance = this;
    }

    public void SwitchDrawMode()
    {
        switch (drawMode)
        {
            case CircuitComponent.DrawMode.Model:
                drawMode = CircuitComponent.DrawMode.Icon;
                buttonText.text = "2D";
                break;
            case CircuitComponent.DrawMode.Icon:
                drawMode = CircuitComponent.DrawMode.Model;
                buttonText.text = "3D";
                break;
        }

        List<CircuitComponent> components = new();
        components.AddRange(FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None));

        foreach (CircuitComponent component in components)
        {
            component.SwitchDrawMode(drawMode);
        }

        onDrawModeSwitch.Invoke();
    }
}
