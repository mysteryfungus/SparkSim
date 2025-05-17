using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConnectionButton : MonoBehaviour
{
    private Button button;
    private Contact contact;
    private Vector3 originalScale;
    [SerializeField] private Image icon;
    private Color originalColor;
    [SerializeField] private Color highlightedColor;
    void Start()
    {
        contact = GetComponentInParent<Contact>();
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        ConnectionManager.instance.onConnection.AddListener(ResetColor);
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        originalColor = icon.color;
    }

    private void OnButtonClick()
    {
        icon.DOColor(highlightedColor, 0.25f);
        ConnectionManager.instance.Connect(contact);
    }

    public void Show()
    {
        transform.DOScale(originalScale, 0.5f);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.5f);
    }

    public void ResetColor()
    {
        icon.DOColor(originalColor, 0.25f);
    }
}
