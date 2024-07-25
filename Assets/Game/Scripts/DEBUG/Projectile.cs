using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables

    protected Vector3 startPosition;
    protected Vector3 endPosition;
    protected Vector3 archPeak;
    protected float speed;
    protected float time = 0f;
    protected bool flight = false;

    [SerializeField] float height = 10f;
    [SerializeField] float moveSpeed = 1f;

    #endregion

    #region UnityMethod

    private void Update()
    {
        if (flight == false)
        {
            transform.position = new Vector3(24, 2.5f, 29);
            Launch(new Vector3(9, 0.65f, 29), height, moveSpeed);
        }

        if(flight)
        {
            if(time >= 1f)
            {
                time = 0f;
                flight = false;
                //Destroy(gameObject);
                return;
            }

            time += speed * Time.deltaTime;

            Vector3 startToPeak = Vector3.Lerp(startPosition, archPeak, time);
            Vector3 peakToEnd = Vector3.Lerp(archPeak, endPosition, time);

            transform.position = Vector3.Lerp(startToPeak, peakToEnd, time);
            transform.forward = Vector3.Lerp(startToPeak, peakToEnd, time + 0.001f) - transform.position;
        }
    }

    #endregion

    #region CustomMethods

    public void Launch(Vector3 endPos, float peakHeight, float moveSpeed)
    {
        startPosition = transform.position;
        endPosition = endPos;
        archPeak = (endPos - startPosition) / 2 + transform.position;
        archPeak.y += peakHeight;

        speed = moveSpeed;

        flight = true;
    }

    #endregion
}
