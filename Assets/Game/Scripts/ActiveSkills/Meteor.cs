using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject finalVFX;

    [Range(1f, 20f)]
    [SerializeField] private float fallSpeed = 10f;

    [HideInInspector] public Tile tileToEffect;

    #endregion

    #region UnityMethods

    void Update()
    {
        if(transform.position.y > 0.5f)
        {
            transform.position += -transform.up * fallSpeed * Time.deltaTime;
        }
        else
        {
            if (finalVFX == null)
            {
                Debug.Log("Meteor was never provided a VFX to spawn");
                Destroy(gameObject);
            }
            else if(tileToEffect == null)
            {
                Debug.Log("Meteor was never provided a tile to effect or it was destroyed");
                Destroy(gameObject);
            }

            Destroy(Instantiate(finalVFX, tileToEffect.transform.position, Quaternion.identity), 2f);
            Destroy(gameObject);
        }
    }

    #endregion
}
