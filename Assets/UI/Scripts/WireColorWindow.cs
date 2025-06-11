using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WireColorWindow : MonoBehaviour
{
    public SelectedMode mode = SelectedMode.Shuffle;
    private float InitialX;
    [SerializeField] List<Button> buttons;
    void Start()
    {
        InitialX = transform.position.x;
        CurrentButton.interactable = false;
    }

    public void ChangeMode(int mode)
    {
        CurrentButton.interactable = true;
        this.mode = (SelectedMode)mode;
        CurrentButton.interactable = false;
    }

    private Button CurrentButton => buttons[(int)mode];

    public void Show()
    {
        transform.DOMoveX(50f, 0.5f);
    }

    public void Hide()
    {
        transform.DOMoveX(InitialX, 0.5f);
    }

    public enum SelectedMode
    {
        Shuffle,
        Red,
        Orange,
        Yellow,
        Green,
        LightBlue,
        Blue,
        Purple,
        White,
        Black
    }
}
