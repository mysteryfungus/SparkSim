using UnityEngine;

public class Billboard : MonoBehaviour
{
    GameObject camera;

    void Start()
    {
        camera = Camera.main.gameObject;
    }

    void LateUpdate()
    {
        transform.LookAt(camera.transform);
    }
}
