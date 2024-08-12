using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireCam : MonoBehaviour
{
    #region Variables

    [SerializeField] float zMax = 15f;
    [SerializeField] float zMin = 5f;
    [SerializeField] float accelSpeed = 2f;
    [SerializeField] float maxSpeed = 2f;

    private bool moveForward = true;
    private float currentSpeed = 0f;

    #endregion

    #region UnityMethods

    void Update()
    {
        if(moveForward && transform.position.z < zMax)
        {
            currentSpeed += accelSpeed * Time.deltaTime;
            if(currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
        else if(transform.position.z > zMin)
        {
            moveForward = false;
            currentSpeed -= accelSpeed * Time.deltaTime;

            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }
        else
        {
            moveForward = true;
        }

        Vector3 newPosition = transform.position;
        newPosition.z += currentSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    #endregion
}
