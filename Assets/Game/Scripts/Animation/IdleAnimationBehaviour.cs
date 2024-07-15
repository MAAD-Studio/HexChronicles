using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimationBehaviour : StateMachineBehaviour
{
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private Vector2 idleTimeRange = new Vector2(4f, 12f);
    private float currentTime = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentTime = 0f;
        idleTime = UnityEngine.Random.Range(idleTimeRange.x, idleTimeRange.y);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentTime += Time.deltaTime;

        if (currentTime >= idleTime)
        {
            animator.SetTrigger("special idle");
            currentTime = 0f;
            idleTime = UnityEngine.Random.Range(idleTimeRange.x, idleTimeRange.y);
        }
    }
}
