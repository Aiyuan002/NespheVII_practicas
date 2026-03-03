using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntermedioTeletransporte : StateMachineBehaviour
{
    public GameObject[] waypoints;

    private Transform player;

    GameObject waypointTP;
    private bool hasTeleported;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        hasTeleported = false;
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = GameObject.Find("WaypointTP_" + (i + 1));
        }

        waypointTP = GetFarthessWaypoint(animator.transform.position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        if (stateInfo.normalizedTime >= 0.5f && !hasTeleported)
        {
            animator.transform.position = waypointTP.transform.position;
            hasTeleported = true;
        }
    }

    private GameObject GetFarthessWaypoint(Vector3 bossIntermedio)
    {
        GameObject farthestWaypoint = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject wp in waypoints)
        {
            float distance = Vector3.Distance(
                player.position, // Posición del enemigo
                wp.transform.position // Posición del waypoint
            );
            Debug.Log("esto si o no?" + distance);
            if (distance < minDistance)
            {
                minDistance = distance;
                farthestWaypoint = wp;
            }
        }
        return farthestWaypoint;
    }
}
