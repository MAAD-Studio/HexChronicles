using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Variables

    private Camera mainCamera;
    private Transform cameraTransform;
    private Vector3 targetPosition;

    [HideInInspector] public bool controlEnabled = true;
    private Transform targetToFollow = null;
    private Vector2 previousMousePos = Vector2.zero;

    [Header("Cursor Textures:")]
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorGrab;

    [Header("Camera Mode:")]
    [SerializeField] private bool useAutoZoomCam = false;

    [Header("Camera Default Positioning: ")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    [Header("Camera Movement:")]
    [SerializeField] private float wasdSpeed = 5f;
    [SerializeField] private float panSpeed = 10f;

    [Header("Lerp: ")]
    [Range(2f, 5f)]
    [SerializeField] private float lerpLevel = 3f;

    [Range(0.02f, 0.1f)]
    [SerializeField] private float lerpDeadZone = 0.05f;

    [Header("Camera Zoom:")]
    [SerializeField] private float scrollZoomSpeed = 4f;
    [SerializeField] private Vector3 onZoomAddOn = Vector3.zero;

    [Header("Camera Limits: ")]
    [SerializeField] private float maxLeft = -5;
    [SerializeField] private float maxRight = 20;

    [SerializeField] private float maxBackward = -5;
    [SerializeField] private float maxForward = 20;

    [Range(0f, 20f)]
    [SerializeField] private float maxZoomIn = 6;

    [Range(10f, 40f)]
    [SerializeField] private float maxZoomOut = 25;

    #endregion

    #region UnityMethods

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        cameraTransform = mainCamera.transform;

        cameraTransform.position = defaultPosition;
        cameraTransform.eulerAngles = defaultRotation;

        targetPosition = cameraTransform.position;

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        ApproachTargetPosition();

        if (controlEnabled)
        {
            CheckKeyInput();

            ZoomUpdate();
            MovementUpdate();

            ClampTarget();
        }
    }

    #endregion

    #region CustomMethods

    /*
     * Moves the Camera towards a target position
     */
    private void ApproachTargetPosition()
    {
        //If a transform has been provided to follow the target is set to its location
        if(targetToFollow != null)
        {
            targetPosition = targetToFollow.position + onZoomAddOn;
        }

        if (Vector3.Distance(cameraTransform.position, targetPosition) > lerpDeadZone)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, lerpLevel * Time.deltaTime);
        }
    }

    /*
     * Checks for any Key Inputs
     */
    private void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            targetPosition = defaultPosition;
        }
    }

    /*
     * Checks if the Player is trying to zoom the Camera
     */
    private void ZoomUpdate()
    {
        float scrollDelta = -Input.mouseScrollDelta.y;

        //Only allows zooming the Camera is within game bounds
        if (scrollDelta < 0 && targetPosition.y > maxZoomIn || scrollDelta > 0 && targetPosition.y < maxZoomOut)
        {
            Vector3 zoomMovement = Vector3.zero;
            zoomMovement.z -= scrollDelta;
            zoomMovement.y += scrollDelta * 2;

            zoomMovement.Normalize();
            zoomMovement *= 24;

            targetPosition += zoomMovement * scrollZoomSpeed * Time.deltaTime;
        }
    }

    /*
     * Checks if the Player is trying to move the Camera
     */
    private void MovementUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.SetCursor(cursorGrab, Vector2.zero, CursorMode.Auto);
            PanMovement();
            previousMousePos = Input.mousePosition;
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            WASDMovement();
        }
    }

    /*
     * Performs Pan style movement on the Camera
     */
    private void PanMovement()
    {
        Vector3 movement = Vector3.zero;

        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        float yScaler = cameraTransform.position.y;

        Vector2 mousePos = Input.mousePosition;

        if (previousMousePos.x != mousePos.x)
        {
            movement.x -= axisX * yScaler;
        }
        if(previousMousePos.y != mousePos.y)
        {
            movement.z -= axisY * yScaler;
        }

        targetPosition += movement * panSpeed * Time.deltaTime;
    }

    /*
     * Performs WASD style movement on the Camera
     */
    private void WASDMovement()
    {
        Vector3 movement = Vector3.zero;

        float yScaler = cameraTransform.position.y;

        if (Input.GetKey(KeyCode.W))
        {
            movement.z += 0.25f * yScaler;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement.z -= 0.25f * yScaler;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= 0.25f * yScaler;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 0.25f * yScaler;
        }

        movement = Vector3.ClampMagnitude(movement, 0.25f * yScaler);

        targetPosition += movement * wasdSpeed * Time.deltaTime;
    }

    /*
     * Clamps targetPosition to keep it within the game bounds
     */
    private void ClampTarget()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);
        targetPosition.z = Mathf.Clamp(targetPosition.z, maxBackward, maxForward);

        targetPosition.y = Mathf.Clamp(targetPosition.y, maxZoomIn, maxZoomOut);
    }

    /*
     * Moves the Camera to the given position
     */
    public void MoveToTargetPosition(Vector3 newTargetPos, bool forceMovement)
    {
        if (useAutoZoomCam || forceMovement)
        {
            targetPosition = newTargetPos + onZoomAddOn;
        }
    }

    /*
     * Moves the Camera back to the default position
     */
    public void MoveToDefault(bool forceMovement)
    {
        if (useAutoZoomCam || forceMovement)
        {
            targetPosition = defaultPosition;
        }
    }

    /*
     * Holds onto a Transform to continuously follow
     */
    public void FollowTarget(Transform target, bool forceMovement)
    {
        if(useAutoZoomCam || forceMovement)
        {
            targetToFollow = target;
        }
    }

    /*
     * Moves in for a close up on character death
     */
    public void MoveToDeathPosition(Transform target, bool forceMovement)
    {
        if (useAutoZoomCam || forceMovement)
        {
            targetPosition = target.position + new Vector3(0, 5, -2);
        }
    }

    /*
     * Stops following any Transforms
     */
    public void StopFollowingTarget()
    {
        targetToFollow = null;
    }

    #endregion
}
