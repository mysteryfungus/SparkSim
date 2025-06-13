using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    Vector2 movementDirection;
    private bool sprinting = false;
    private Transform _transform;

    public Camera mainCamera;
    public Camera altCamera;

    [SerializeField] private Transform _rotationPoint;
    private float _targetRotation;
    public float movementSpeed = 1f;
    public float sprintSpeed = 2f;
    private float angularVelocity;

    [SerializeField] private float rotationDuration = 0.1f;
    [SerializeField] private float rotationStep = 90f;

    void Awake()
    {
        _transform = transform;
        _targetRotation = _rotationPoint.eulerAngles.y;
        altCamera.enabled = false;
    }

    public void SwitchPerspective()
    {
        CircuitComponent.DrawMode mode = DrawModeSwitcher.instance.drawMode;
        switch (mode)
        {
            case CircuitComponent.DrawMode.Model:
                mainCamera.enabled = true;
                altCamera.enabled = false;
                break;

            case CircuitComponent.DrawMode.Icon:
                mainCamera.enabled = false;
                altCamera.enabled = true;
                break;
        }
    }

    void FixedUpdate()
    {
        // Двигаем камеру
        MoveCamera(sprinting ? sprintSpeed : movementSpeed);

        // Плавно вращаем камеру
        SmoothRotate();
    }

    void MoveCamera(float speed)
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 movement = speed * Time.fixedDeltaTime * (forward * movementDirection.y + right * movementDirection.x);
        _rotationPoint.Translate(movement.x, 0, movement.z, Space.World);
    }

    void RotateCamera(float direction)
    {
        // Определяем направление вращения
        if (direction < 0)
            _targetRotation += rotationStep;
        else if (direction > 0)
            _targetRotation -= rotationStep;
    }

    public void CameraInput(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        sprinting = context.started;
    }

    public void CameraRotationInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RotateCamera(context.ReadValue<float>());
        }
    }

    void SmoothRotate()
    {
        _rotationPoint.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(
            _rotationPoint.eulerAngles.y,
            _targetRotation,
            ref angularVelocity,
            rotationDuration
        ), 0f);
    }
}
