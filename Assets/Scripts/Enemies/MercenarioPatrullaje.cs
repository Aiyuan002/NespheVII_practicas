using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenarioPatrullaje : StateMachineBehaviour
{
    public float moveSpeed;

    private MercenarioBehaviour mercenario;
    private float stopTimer = 0f;
    public int stopTime;
    private RobotWallChecker childrenWallC;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        childrenWallC = animator.gameObject.GetComponentInChildren<RobotWallChecker>();
        mercenario = animator.gameObject.GetComponent<MercenarioBehaviour>();
        mercenario.isFollow = false;
        stopTime = Random.Range(1, 10);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        if (!mercenario.isWaiting)
        {
            stopTimer += Time.deltaTime;
            if (mercenario.lookRight)
            {
                mercenario.transform.position = Vector2.MoveTowards(
                    mercenario.transform.position,
                    new Vector2(
                        mercenario.transform.position.x - 1f,
                        mercenario.transform.position.y
                    ),
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                mercenario.transform.position = Vector2.MoveTowards(
                    mercenario.transform.position,
                    new Vector2(
                        mercenario.transform.position.x + 1f,
                        mercenario.transform.position.y
                    ),
                    moveSpeed * Time.deltaTime
                );
            }
            if (childrenWallC.isWall)
            {
                mercenario.ChangeDirectionPatrol();
            }
            if (stopTimer >= stopTime)
            {
                mercenario.ChangeDirectionPatrol();
                animator.SetBool("Wait", true);
                stopTimer = 0;
            }
        }
    }
    /* public IEnumerator WaitAndChangeDirection(bool isWaiting, float stopTimer, bool lookRight)
     {
         isWaiting = true;
         stopTimer = 0f; // Reinicia el temporizador
 
         yield return new WaitForSeconds(1f); // Espera 1 segundo
 
         lookRight = false;
         isWaiting = false;
     }*/
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
