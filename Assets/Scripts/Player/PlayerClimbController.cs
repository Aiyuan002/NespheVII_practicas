using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    public PlayerController pc;


    public void StartClimbFinished()
    {
        pc.canMove = true;
    }
}
