using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject activePanel;
    public void SwitchPanel(GameObject panel)
    {
        activePanel.SetActive(false);
        panel.SetActive(true);
        activePanel = panel;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
