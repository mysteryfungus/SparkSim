using UnityEngine;
using UnityEngine.UI;

public class SimulationButton : MonoBehaviour
{
    private State state = State.Idle;
    [SerializeField] Text text;
    enum State
    {
        Idle,
        Running
    }
    public void SimulationToggle()
    {
        switch (state)
        {
            case State.Idle:
                SimulationManager.instance.StartSimulation();
                text.text = "Стоп";
                break;

            case State.Running:
                SimulationManager.instance.StopSimulation();
                text.text = "Запуск";
                break;
        }
    }
}
