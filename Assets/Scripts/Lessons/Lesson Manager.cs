using UnityEngine;
using UnityEngine.Events;

public class LessonManager : MonoBehaviour
{
    public enum LessonState{
        Theory,
        Practice,
        Completed
    }
    public LessonState _lessonState;
    public UnityEvent OnLessonStateChanged;
    void Start()
    {
        _lessonState = LessonState.Theory;
    }

    public void ChangeLessonState(LessonState newState){
        _lessonState = newState;
        OnLessonStateChanged?.Invoke();
    }
}
