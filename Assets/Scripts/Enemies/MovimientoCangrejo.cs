using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCangrejo : StateMachineBehaviour
{
    private Transform cangrejoTransform;
    public float speed = 2f;
    private Vector3 puntoDestino;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        // Inicializa referencias
        cangrejoTransform = animator.transform;

        // Define un punto aleatorio para patrullar
        puntoDestino =
            cangrejoTransform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Debug.Log("Entrando en estado de patrullaje.");
    }

    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        // Mover hacia el punto de patrullaje
        cangrejoTransform.position = Vector3.MoveTowards(
            cangrejoTransform.position,
            puntoDestino,
            speed * Time.deltaTime
        );

        // Si llega al punto, cambiar de estado (puedes usar un parámetro en el Animator)
        if (Vector3.Distance(cangrejoTransform.position, puntoDestino) < 0.1f)
        {
            animator.SetTrigger("CambioEstado"); // Cambiar a otro estado
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
