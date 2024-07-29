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

    protected Character character;
    protected float damage = 0f;

    [SerializeField] GameObject endEffect;

    #endregion

    #region UnityMethod

    private void Update()
    {
        if(flight)
        {
            if(time >= 1f)
            {
                time = 0f;
                flight = false;
                
                GameObject deathEffect = Instantiate(endEffect, transform.position, Quaternion.identity);
                Destroy(deathEffect, 0.5f);

                if(character != null)
                {
                    character.TakeDamage(damage, ElementType.Base);
                }

                Destroy(gameObject);
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

    public void Launch(Vector3 endPos, float peakHeight, float moveSpeed, Character characterToEffect, float projectileDamage)
    {
        startPosition = transform.position;
        endPosition = endPos;
        archPeak = (endPos - startPosition) / 2 + transform.position;
        archPeak.y += peakHeight;

        speed = moveSpeed;

        flight = true;

        character = characterToEffect;
        damage = projectileDamage;
    }

    #endregion
}
