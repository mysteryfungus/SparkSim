using UnityEngine;

public class Billboard : MonoBehaviour
{
    GameObject sceneCamera;

    void Start()
    {
        sceneCamera = Camera.main.gameObject;
    }

    void LateUpdate()
    {
        transform.LookAt(sceneCamera.transform);
    }
}
