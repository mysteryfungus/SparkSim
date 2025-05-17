using UnityEngine;

public class DeletionManager : MonoBehaviour
{
    public static DeletionManager instance;

    private bool controlsShown = false;

    private void Awake()
    {
        Debug.Log("DeletionManager Awake");
        instance = this;
    }

    public void EnterDeletionMode()
    {
        Debug.Log("Entering deletion mode");
        if(controlsShown) return;

        foreach(var wireDeletionButton in FindObjectsOfType<WireDeletionButton>())
        {
            wireDeletionButton.Show();
        }

        controlsShown = true;
    }

    public void ExitDeletionMode()
    {
        Debug.Log("Exiting deletion mode");
        if(!controlsShown) return;

        foreach(var wireDeletionButton in FindObjectsOfType<WireDeletionButton>())
        {
            wireDeletionButton.Hide();
        }

        controlsShown = false;
    }

}
