using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrullajeCangrejo : StateMachineBehaviour
{
    public float moveSpeed;
    private Transform player;

    private BossCangrejo bossCangrejo;
    private float stopTimer = 0f;
    public float stopTime = 2f;
    private RobotWallChecker childrenWallC;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        childrenWallC = animator.gameObject.GetComponentInChildren<RobotWallChecker>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bossCangrejo = animator.gameObject.GetComponent<BossCangrejo>();
        bossCangrejo.isFollow = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        stopTimer += Time.deltaTime;
        if (bossCangrejo.lookRight)
        {
            bossCangrejo.transform.position = Vector2.MoveTowards(
                bossCangrejo.transform.position,
                new Vector2(
                    bossCangrejo.transform.position.x - 1f,
                    bossCangrejo.transform.position.y
                ),
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            bossCangrejo.transform.position = Vector2.MoveTowards(
                bossCangrejo.transform.position,
                new Vector2(
                    bossCangrejo.transform.position.x + 1f,
                    bossCangrejo.transform.position.y
                ),
                moveSpeed * Time.deltaTime
            );
        }
        if (childrenWallC.isWall)
        {
            bossCangrejo.ChangeDirectionPatrol();
            stopTimer = 0f;
        }
        if (stopTimer >= stopTime)
        {
            bossCangrejo.ChangeDirectionPatrol();
            stopTimer = 0f;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
