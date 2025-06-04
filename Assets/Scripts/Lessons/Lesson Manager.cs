using UnityEngine;
using System.Collections.Generic;

public class LessonManager : MonoBehaviour
{
    public static LessonManager instance;

    public enum LessonState{
        Theory,
        Practice,
    }
    public LessonState _lessonState;
    public List<GameObject> _theoryPanels;
    public List<GameObject> _practicePanels;
    void Start()
    {
        instance = this;
        ChangeLessonState(LessonState.Theory);
    }

    public void SwitchToTheory(){
        ChangeLessonState(LessonState.Theory);
    }

    public void SwitchToPractice(){
        ChangeLessonState(LessonState.Practice);
    }

    private void ChangeLessonState(LessonState newState){
        _lessonState = newState;
        
        switch (_lessonState){
            case LessonState.Theory:
                foreach (var panel in _practicePanels){
                    panel.SetActive(false);
                }

                foreach (var panel in _theoryPanels){
                    panel.SetActive(true);
                }
                break;
            case LessonState.Practice:
                foreach (var panel in _practicePanels){
                    panel.SetActive(true);
                }

                foreach (var panel in _theoryPanels){
                    panel.SetActive(false);
                }
                break;
        }
    }
}
