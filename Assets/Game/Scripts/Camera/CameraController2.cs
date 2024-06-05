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

    [Header("Camera Default Positioning: ")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    [Header("Camera Movement:")]
    [SerializeField] private float wasdSpeed = 5f;
    [SerializeField] private float panSpeed = 10f;

    [Header("Camera Zoom:")]
    [SerializeField] private float scrollZoomSpeed = 4f;

    [Header("Camera Limits: ")]
    [SerializeField] private float maxLeft = -5;
    [SerializeField] private float maxRight = 20;

    [SerializeField] private float maxBackward = -5;
    [SerializeField] private float maxForward = 20;

    [SerializeField] private float maxZoomIn = 6;
    [SerializeField] private float maxZoomOut = 25;

    Vector3 previousMousePos = Vector3.zero;

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
            PanMovement();
        }
        else
        {
            WASDMovement();
        }

        previousMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void PanMovement()
    {
        Vector3 movement = Vector3.zero;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log("--------------------------");
        Debug.Log(mousePosition.x);
        Debug.Log(previousMousePos.x);
        Debug.Log("--------------------------");

        float movementX = mousePosition.x - previousMousePos.x;
        float movementY = mousePosition.y - previousMousePos.y;

        Debug.Log("X: " + movementX);
        Debug.Log("Y: " + movementY);

        movement.x = movementX;
        movement.z = movementY;

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

    #endregion
}
