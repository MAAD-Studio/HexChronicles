using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController2 : MonoBehaviour
{
    #region Variables

    private Camera mainCamera;
    private Transform cameraTransform;
    private Vector3 targetPosition;

    [HideInInspector] public bool controlEnabled = true;

    [Header("Cursor Textures:")]
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorGrab;

    [Header("Camera Default Positioning: ")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    [Header("Camera Movement:")]
    [SerializeField] private float wasdSpeed = 5f;
    [SerializeField] private float panSpeed = 10f;

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
    }

    private void Update()
    {
        ApproachTargetPosition();

        if(controlEnabled)
        {
            CheckInput();

            ZoomUpdate();
            MovementUpdate();

            ClampTarget();
        }
    }

    #endregion

    #region CustomMethods

    private void ApproachTargetPosition()
    {
        if(Vector3.Distance(cameraTransform.position, targetPosition) > 0.05f)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, 0.035f);
        }
    }

    private void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            targetPosition = defaultPosition;
        }
    }

    private void ZoomUpdate()
    {
        float scrollDelta = -Input.mouseScrollDelta.y;
        
        if(scrollDelta < 0 && targetPosition.y > maxZoomIn || scrollDelta > 0 && targetPosition.y < maxZoomOut)
        {
            Vector3 zoomMovement = Vector3.zero;
            zoomMovement.z -= scrollDelta;
            zoomMovement.y += scrollDelta * 2;

            zoomMovement.Normalize();
            zoomMovement *= 24;

            targetPosition += zoomMovement * scrollZoomSpeed * Time.deltaTime;
        }
    }
     
    private void MovementUpdate()
    {
        if(Input.GetMouseButton(2))
        {
            Cursor.SetCursor(cursorGrab, Vector2.zero, CursorMode.Auto);
            PanMovement();
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            WASDMovement();
        }
    }

    private void PanMovement()
    {
        Vector3 movement = Vector3.zero;

        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        float yScaler = cameraTransform.position.y;

        movement.x -= axisX * panSpeed * yScaler;
        movement.z -= axisY * panSpeed * yScaler;

        targetPosition += movement * Time.deltaTime;
    }

    private void WASDMovement()
    {
        Vector3 movement = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
        {
            movement.z += 1f;
        }

        if(Input.GetKey(KeyCode.S))
        {
            movement.z -= 1f;
        }

        if(Input.GetKey(KeyCode.A))
        {
            movement.x -= 1f;
        }

        if(Input.GetKey(KeyCode.D))
        {
            movement.x += 1f;
        }

        movement.Normalize();

        targetPosition += movement * wasdSpeed * Time.deltaTime;
    }

    private void ClampTarget()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);
        targetPosition.z = Mathf.Clamp(targetPosition.z, maxBackward, maxForward);

        targetPosition.y = Mathf.Clamp(targetPosition.y, maxZoomIn, maxZoomOut);
    }

    private void SetTargetPosition(Vector3 newTargetPos)
    {
        targetPosition = newTargetPos + onZoomAddOn;
    }

    #endregion
}
