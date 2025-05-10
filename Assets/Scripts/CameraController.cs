using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float shiftMultiplier = 2f;
    [SerializeField] private float moveSmoothing = 0.1f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationAngle = 45f;
    [SerializeField] private float rotationSmoothing = 0.1f;
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 20f;
    [SerializeField] private float zoomSmoothing = 0.1f;
    [SerializeField] private float defaultLookDistance = 10f;
    
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    
    private float currentZoomDistance = 10f;
    private float targetZoomDistance;
    
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private Vector3 lookPoint;
    private float lookDistance;
    
    private Vector3 currentZoomPosition;
    private Vector3 targetZoomPosition;
    
    private void Start()
    {
        // Initialize position and rotation
        currentPosition = transform.position;
        targetPosition = currentPosition;
        currentZoomPosition = currentPosition;
        targetZoomPosition = currentPosition;
        targetRotation = currentRotation;
        targetZoomDistance = currentZoomDistance;
        lookDistance = defaultLookDistance;
        
        // Calculate initial look point
        UpdateLookPoint();
        
        // Set initial camera position
        UpdateCameraTransform();
    }
    
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        UpdateCameraTransform();
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            // Calculate movement speed
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= shiftMultiplier;
            }
            
            // Calculate movement direction in world space
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 movement = (forward * vertical + right * horizontal) * speed * Time.deltaTime;
            targetPosition += movement;
            targetZoomPosition += movement;
            lookPoint += movement; // Move look point with camera
        }
        
        // Smooth movement
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, 1f - Mathf.Pow(moveSmoothing, Time.deltaTime * 60f));
    }
    
    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetRotation -= rotationAngle;
            UpdateLookPointAfterRotation();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            targetRotation += rotationAngle;
            UpdateLookPointAfterRotation();
        }
        
        // Smooth rotation
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, 1f - Mathf.Pow(rotationSmoothing, Time.deltaTime * 60f));
    }
    
    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            // Calculate new zoom distance
            float zoomDelta = scrollInput * zoomSpeed;
            targetZoomDistance -= zoomDelta;
            targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
            
            // Calculate direction to look point
            Vector3 directionToLookPoint = (lookPoint - currentPosition).normalized;
            
            // Update target zoom position
            targetZoomPosition += directionToLookPoint * zoomDelta;
        }
        
        // Smooth zoom position
        currentZoomPosition = Vector3.Lerp(currentZoomPosition, targetZoomPosition, 1f - Mathf.Pow(zoomSmoothing, Time.deltaTime * 60f));
        
        // Smooth zoom distance
        currentZoomDistance = Mathf.Lerp(currentZoomDistance, targetZoomDistance, 1f - Mathf.Pow(zoomSmoothing, Time.deltaTime * 60f));
    }
    
    private void UpdateLookPoint()
    {
        // Calculate look point based on camera's forward direction and current distance
        Quaternion rotation = Quaternion.Euler(45f, currentRotation, 0);
        Vector3 forward = rotation * Vector3.forward;
        lookPoint = currentPosition + forward * lookDistance;
    }
    
    private void UpdateLookPointAfterRotation()
    {
        // Calculate new look point after rotation
        Quaternion rotation = Quaternion.Euler(45f, targetRotation, 0);
        Vector3 forward = rotation * Vector3.forward;
        lookPoint = currentPosition + forward * lookDistance;
    }
    
    private void UpdateCameraTransform()
    {
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(45f, currentRotation, 0);
        
        // Calculate position using smoothed zoom position
        Vector3 position = currentZoomPosition;
        position.y = currentZoomDistance * Mathf.Sin(45f * Mathf.Deg2Rad);
        
        // Apply to transform
        transform.rotation = rotation;
        transform.position = position;
        
        // Update look point if it's too far
        if (Vector3.Distance(lookPoint, position) > maxZoomDistance * 2)
        {
            UpdateLookPoint();
        }
    }
} 