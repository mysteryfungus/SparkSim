using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WireDeletionButton : MonoBehaviour
{
    private Button button;
    private Wire wire;
    private Vector3 originalScale;
    void Start()
    {
        wire = GetComponentInParent<Wire>();
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnButtonClick()
    {
        wire.RemoveConnection();
    }

    public void Show()
    {
        transform.DOScale(originalScale, 0.5f);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.5f);
    }
}
