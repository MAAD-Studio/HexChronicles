using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffects : MonoBehaviour
{
    #region Variables

    [SerializeField] private AudioSO clickSound;

    #endregion

    #region CustomMethods

    public void PlayClickSound()
    {
        if(clickSound != null)
        {
            AudioManager.Instance.PlaySound(clickSound);
        }
        else
        {
            Debug.Log("*BUTTON WAS GIVEN NO CLICK SOUND*");
        }
    }

    #endregion
}
