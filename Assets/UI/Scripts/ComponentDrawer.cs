using DG.Tweening;
using UnityEngine;

public class ComponentDrawer : MonoBehaviour
{
    private float InitialY;
    public float SlideDuration = 0.5f;

    void Awake()
    {
        InitialY = transform.position.y;
    }

    public void SlideIn()
    {
        transform.DOMoveY(45, SlideDuration);
    }

    public void SlideOut()
    {
        transform.DOMoveY(InitialY, SlideDuration);
    }
}
