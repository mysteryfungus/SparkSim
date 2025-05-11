using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public void LoadScene(string name)
    {
        Debug.Log($"Started loading scene {name}");
        SceneManager.LoadScene(name);
    }

    public void AppQuit()
    {
        Debug.Log("Quitting the app...");
        Application.Quit(0);
    }
}
