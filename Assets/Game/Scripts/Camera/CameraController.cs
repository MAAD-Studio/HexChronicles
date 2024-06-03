using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Variables

    private Camera mainCamera;
    private Transform cameraTransform;

    [HideInInspector] public bool allowControl = true;

    [Header("Camera Default Info:")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    private Vector3 cameraPosition = Vector3.zero;

    [Header("Camera WASD Controls: ")]
    [SerializeField] private float maxMoveSpeed = 10f;
    [SerializeField] private float accelerationSpeed = 18f;
    [SerializeField] private float deccelerationSpeed = 12f;

    [Header("Camera MiddleMouse Controls: ")]
    [SerializeField] private float panSpeed = 10f;

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

    private Character selectedCharacter;
    private TileObject selectedObject;

    [Header("Camera Left-Right Limits: ")]
    [SerializeField] private int maxLeft = -5;
    [SerializeField] private int maxRight = 20;

    [Header("Camera Forward-Back Limits: ")]
    [SerializeField] private int maxBackwards = -5;
    [SerializeField] private int maxForwards = 20;

    [Header("Camera Zoom Limits: ")]
    [SerializeField] private int maxZoomIn = 6;
    [SerializeField] private int maxZoomOut = 20;

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
        ApproachTargetPosition();

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

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, maxLeft, maxRight);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, maxBackwards, maxForwards);

            targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);
            targetPosition.z = Mathf.Clamp(targetPosition.z, maxBackwards, maxForwards);
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
            if (scrollDelta < 0 && targetPosition.y > maxZoomIn || scrollDelta > 0 && targetPosition.y < maxZoomOut)
            {
                targetPosition += -mainCameraForward * scrollDelta;
                targetPosition.y += scrollDelta;
            }
        }
    }

    //Checks for WASD Key inputs
    public void MoveUpdate(Vector3 mainCameraForward, Vector3 mainCameraRight)
    {
        if(Input.GetMouseButton(2))
        {
            MoveMiddleMouse(mainCameraForward, mainCameraRight);
        }
        else
        {
            MoveKeyboard(mainCameraForward, mainCameraRight);
        }
    }

    private void MoveKeyboard(Vector3 mainCameraForward, Vector3 mainCameraRight)
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

        //Adds the velocity and clamps to ensure the player isn't leaving the map
        velocityX = Vector3.ClampMagnitude(velocityX, maxMoveSpeed);
        velocityZ = Vector3.ClampMagnitude(velocityZ, maxMoveSpeed);

        totalVelocity = velocityX + velocityZ;

        cameraPosition += totalVelocity * Time.deltaTime;
        targetPosition += totalVelocity * Time.deltaTime;
    }

    private void MoveMiddleMouse(Vector3 mainCameraForward, Vector3 mainCameraRight)
    {
        Vector3 movement = Vector3.zero;

        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        if (axisX < 0)
        {
            movement.x -= panSpeed * (axisX * 5) * (cameraPosition.y / 10) * Time.deltaTime;
        }
        else if(axisX > 0)
        {
            movement.x -= panSpeed * (axisX * 5) * (cameraPosition.y / 10) * Time.deltaTime;
        }

        if(axisY < 0)
        {
            movement.z -= panSpeed * (axisY * 5) * (cameraPosition.y / 10) * Time.deltaTime;
        }
        else if(axisY > 0)
        {
            movement.z -= panSpeed * (axisY * 5) * (cameraPosition.y / 10) * Time.deltaTime;
        }

        movement = Vector3.ClampMagnitude(movement, maxMoveSpeed);

        cameraPosition += movement;
        targetPosition += movement;
    }

    //Checks if the player wants to return to a default cam position
    public void DefaultsUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetValues(defaultPosition);
        }
        else if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            if(selectedCharacter != null)
            {
                SetCamToSelectedCharacter(selectedCharacter);
            }
        }
    }

    //Sets a new target position and gets rid of velocity
    private void SetValues(Vector3 position)
    {
        targetPosition = position;
        velocityX = Vector3.zero;
        velocityZ = Vector3.zero;
    }

    //Sets the cam to move to the default position
    public void SetCamToDefault()
    {
        SetValues(defaultPosition);
    }

    //Sets the cam to move to the position of a character
    public void SetCamToSelectedCharacter(Character character)
    {
        Vector3 newPosition = character.transform.position;
        newPosition.y += 10;
        newPosition.z -= 4;

        selectedCharacter = character;

        SetValues(newPosition);
    }

    //Sets the cam to move to the position of a object
    public void SetCamToObject(TileObject tileObject)
    {
        Vector3 newPosition = tileObject.transform.position;
        newPosition.y += 10;
        newPosition.z -= 4;

        selectedObject = tileObject;

        SetValues(newPosition);
    }

    //Tells the camera to unselect the character it is holding onto
    public void UnSelectCharacter()
    {
        selectedCharacter = null;
    }

    //Tells the camera to unselect the object it is holding onto
    public void UnSelectObject()
    {
        selectedObject = null;
    }

    //Gets the camera to approach the target position
    public void ApproachTargetPosition()
    {
        if(selectedCharacter != null && !allowControl)
        {
            SetCamToSelectedCharacter(selectedCharacter);
        }
        else if(selectedObject != null && !allowControl)
        {
            SetCamToObject(selectedObject);
        }

        //Interpolates for smoother movement
        if (Vector3.Distance(cameraPosition, targetPosition) > 0.5f)
        {
            cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, 0.05f);
        }
        else
        {
            cameraPosition = targetPosition;
        }

        cameraTransform.position = cameraPosition;
    }

    #endregion
}
