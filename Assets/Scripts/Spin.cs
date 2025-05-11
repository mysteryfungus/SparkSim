using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Spin : MonoBehaviour
{
    public float speed = 30f;
    public float interval = 5f;

    private Transform obj;
    private float timer = 0f;
    private Vector3[] axes = {Vector3.up, Vector3.right, Vector3.forward};
    private int axisIndex = 0;
    private int reverseRotation = 0;

    void Awake()
    {
        obj = this.transform;
    }

    private float localSpeed => speed * Time.fixedDeltaTime;

    private int GetReverseModifier()
    {
        if(reverseRotation == 0)
        {
            return -1;
        } else {
            return 1;
        }
    } 

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer >= interval)
        {
            if(axisIndex != 2) axisIndex += 1;
            else axisIndex = 0;
            timer = 0f;
            reverseRotation = Random.Range(0, 2);
        }
        obj.Rotate(GetReverseModifier() * axes[axisIndex] * localSpeed);
    }
}
