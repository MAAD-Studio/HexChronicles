using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Variables

    private Camera mainCamera;
    private Transform cameraTransform;

    private bool allowControl = true;

    [Header("Camera Default Info:")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    private Vector3 cameraPosition = Vector3.zero;

    [Header("Camera WASD Controls: ")]
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float accelerationSpeed = 18f;
    [SerializeField] private float deccelerationSpeed = 12f;

    private Vector3 velocityX = Vector3.zero;
    private bool wPressed = false;
    private bool sPressed = false;

    private Vector3 velocityZ = Vector3.zero;
    private bool dPressed = false;
    private bool aPressed = false;

    private Vector3 totalVelocity = Vector3.zero;

    [Header("Camera Zoom Controls: ")]
    [SerializeField] private float zoomSpeed = 2f;
    private Vector3 targetPosition = Vector3.zero;

    #endregion

    #region UnityMethods

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        mainCamera.transform.position = defaultPosition;
        mainCamera.transform.eulerAngles = defaultRotation;

        cameraTransform = mainCamera.transform;
        cameraPosition = cameraTransform.position;
        targetPosition = cameraTransform.position;
    }

    void Update()
    {
        if (allowControl)
        {
            DefaultsUpdate();

            Vector3 mainCameraForward = mainCamera.transform.forward;
            mainCameraForward.y = 0;

            Vector3 mainCameraRight = mainCamera.transform.right;
            mainCameraRight.y = 0;

            //Checks for Scroll Wheel inputs
            ZoomUpdate(mainCameraForward);

            //Checks for WASD Key inputs
            MoveUpdate(mainCameraForward, mainCameraRight);

            //Adds the velocity and clamps to ensure the player isn't leaving the map
            velocityX = Vector3.ClampMagnitude(velocityX, maxMoveSpeed);
            velocityZ = Vector3.ClampMagnitude(velocityZ, maxMoveSpeed);

            totalVelocity = velocityX + velocityZ;

            cameraPosition += totalVelocity * Time.deltaTime;
            targetPosition += totalVelocity * Time.deltaTime;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, -5, 20);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, -5, 20);

            targetPosition.x = Mathf.Clamp(targetPosition.x, -5, 20);
            targetPosition.z = Mathf.Clamp(targetPosition.z, -5, 20);

            cameraTransform.position = cameraPosition;
        }
    }

    #endregion

    #region CustomMethods

    //Decelerates a provided velocity
    public void Decelerate(ref Vector3 velocity, float multiplier)
    {
        if(Vector3.Magnitude(velocity) > 0.05)
        {
            velocity -= velocity.normalized * (deccelerationSpeed * multiplier) * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    //Checks for Scroll Wheel inputs
    public void ZoomUpdate(Vector3 mainCameraForward)
    {
        //Checks if the player is trying to zoom in using the scroll wheel
        float scrollDelta = -Input.mouseScrollDelta.y * zoomSpeed;
        if (scrollDelta != 0)
        {
            //Limits how far out or in the player can zoom
            if (scrollDelta < 0 && targetPosition.y > 6 || scrollDelta > 0 && targetPosition.y < 20)
            {
                targetPosition += -mainCameraForward * scrollDelta;
                targetPosition.y += scrollDelta;
            }
        }

        //Interpolates for smoother zooming
        if (Vector3.Distance(cameraPosition, targetPosition) > 0.5f)
        {
            cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, 0.05f);
        }
        else
        {
            cameraPosition = targetPosition;
        }
    }

    //Checks for WASD Key inputs
    public void MoveUpdate(Vector3 mainCameraForward, Vector3 mainCameraRight)
    {
        if (Input.GetKey(KeyCode.W))
        {
            wPressed = true;
            if (!sPressed)
            {
                //If we are accelerating in the opposite keys direction decelerate
                if (velocityX.magnitude > 0.05 && Vector3.Angle(velocityX, mainCameraForward) > 90)
                {
                    velocityX -= velocityX.normalized * (deccelerationSpeed * 4) * Time.deltaTime;
                }
                else
                {
                    velocityX += mainCameraForward * accelerationSpeed * Time.deltaTime;
                }
            }
            else
            {
                //If both W and S are pressed decelerate
                Decelerate(ref velocityX, 2);
            }
        }
        else
        {
            wPressed = false;
            if (!sPressed)
            {
                //If neither W or S are pressed decelerate
                Decelerate(ref velocityX, 2);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            sPressed = true;
            if (!wPressed)
            {
                //If we are accelerating in the opposite keys direction decelerate
                if (velocityX.magnitude > 0.05 && Vector3.Angle(velocityX, -mainCameraForward) > 90)
                {
                    velocityX -= velocityX.normalized * (deccelerationSpeed * 4) * Time.deltaTime;
                }
                else
                {
                    velocityX -= mainCameraForward * accelerationSpeed * Time.deltaTime;
                }
            }
        }
        else
        {
            sPressed = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dPressed = true;
            if (!aPressed)
            {
                //If we are accelerating in the opposite keys direction decelerate
                if (velocityZ.magnitude > 0.05 && Vector3.Angle(velocityZ, mainCameraRight) > 90)
                {
                    velocityZ -= velocityZ.normalized * (deccelerationSpeed * 4) * Time.deltaTime;
                }
                else
                {
                    velocityZ += mainCameraRight * accelerationSpeed * Time.deltaTime;
                }
            }
            else
            {
                //If both D and A are pressed decelerate
                Decelerate(ref velocityZ, 2);
            }
        }
        else
        {
            dPressed = false;
            if (!aPressed)
            {
                //If neither D or A are pressed decelerate
                Decelerate(ref velocityZ, 2);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            aPressed = true;
            if (!dPressed)
            {
                //If we are accelerating in the opposite keys direction decelerate
                if (velocityZ.magnitude > 0.05 && Vector3.Angle(velocityZ, -mainCameraRight) > 90)
                {
                    velocityZ -= velocityZ.normalized * (deccelerationSpeed * 4) * Time.deltaTime;
                }
                else
                {
                    velocityZ -= mainCameraRight * accelerationSpeed * Time.deltaTime;
                }
            }
        }
        else
        {
            aPressed = false;
        }
    }

    public void DefaultsUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            ResetValues();
        }
    }

    public void ResetValues()
    {
        cameraTransform.position = defaultPosition;
        cameraPosition = defaultPosition;
        targetPosition = defaultPosition;
        velocityX = Vector3.zero;
        velocityZ = Vector3.zero;
    }

    #endregion
}
