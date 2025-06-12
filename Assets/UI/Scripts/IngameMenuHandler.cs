using DevionGames;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenuHandler : MonoBehaviour
{
    private const float Duration = 0.25f / 2;
    private GameObject menuWindow;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject uiBlocker;

    void Start()
    {
        menuWindow = gameObject;
        if (uiBlocker != null) uiBlocker.SetActive(false);
        transform.localScale = new Vector3(0, 0, 0);
    }

    public void Show()
    {
        menuWindow.transform.DOScale(1f, Duration);
        cameraController.enabled = false;
        uiBlocker.SetActive(true);
    }

    public void Hide()
    {
        menuWindow.transform.DOScale(0f, Duration);
        cameraController.enabled = true;
        uiBlocker.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
