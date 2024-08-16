using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly_Base : Enemy_Base
{
    #region Variables

    protected Vector3 startPosition;
    protected Vector3 endPosition;
    protected Vector3 archPeak;
    protected float speed;
    protected float time = 0f;
    protected bool performLaunch = false;
    protected Tile settleDownTile = null;

    protected bool selfDestruct = false;
    protected Enemy_MasterJelly targetJellyMaster = null;
    protected Enemy_KingJelly targetJellyKing = null;
    protected bool archFinished = true;

    #endregion

    #region UnityMethods

    private void Update()
    {
        if(performLaunch)
        {
            if (time >= 1f)
            {
                if (characterTile != null)
                {
                    characterTile.characterOnTile = null;
                    characterTile.tileOccupied = false;

                    characterTile = null;
                }

                Vector3 checkPosition = transform.position + new Vector3(0, 1, 0);
                if (Physics.Raycast(checkPosition, -transform.up, out RaycastHit hit, 50f, tileLayer))
                {
                    FinalizeTileChoice(hit.transform.GetComponent<Tile>());
                }

                healthBar.gameObject.SetActive(true);
                performLaunch = false;

                archFinished = true;

                TurnManager turnManager = FindObjectOfType<TurnManager>();
                if(turnManager == null)
                {
                    Debug.Log("TURNMANAGER NOT FOUND BY ARCH");
                    return;
                }

                if (selfDestruct)
                {
                    DestroySelfEnemy(turnManager);
                }

                if(targetJellyMaster != null)
                {
                    targetJellyMaster.CombineJelly(turnManager);
                }
                else if(targetJellyKing != null)
                {
                    targetJellyKing.CombineJelly();
                }

                return;
            }

            time += speed * Time.deltaTime * GameManager.Instance.GameSpeed;

            Vector3 startToPeak = Vector3.Lerp(startPosition, archPeak, time);
            Vector3 peakToEnd = Vector3.Lerp(archPeak, endPosition, time);

            transform.position = Vector3.Lerp(startToPeak, peakToEnd, time);
            transform.forward = Vector3.Lerp(startToPeak, peakToEnd, time + 0.001f) - transform.position;
        }
    }

    #endregion

    #region CustomMethods

    public void InitiateArch(Vector3 endPos, float launchspeed, float launchHeight)
    {
        healthBar.gameObject.SetActive(false);

        startPosition = transform.position;
        endPosition = endPos;
        archPeak = (endPosition - startPosition) + transform.position;
        archPeak.y += launchHeight;

        speed = launchspeed;
        time = 0f;

        performLaunch = true;

        archFinished = false;
    }

    public void InitiateArch(Vector3 endPos, float launchspeed, float launchHeight, Enemy_MasterJelly target)
    {
        targetJellyMaster = target;

        InitiateArch(endPos, launchspeed, launchHeight);
    }

    public void InitiateArch(Vector3 endPos, float launchspeed, float launchHeight, Enemy_KingJelly target)
    {
        targetJellyKing = target;

        InitiateArch(endPos, launchspeed, launchHeight);
    }

    #endregion
}
