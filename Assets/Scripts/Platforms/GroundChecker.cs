using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundChecker : MonoBehaviour
{
    private CharacterController parent;
    public bool checkGround;

    public float teleportCooldown = 0.2f;
    private bool shouldTeleportToTop = true;

    private float teleportTimer = 0f;

    private void Start()
    {
        parent = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        teleportTimer += Time.deltaTime;
        if(parent.isRoofTouching)
        {
            shouldTeleportToTop = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            bool isAcidTerrain = collision.gameObject.GetComponent<AcidTerrain>() != null;
            if (shouldTeleportToTop && !parent.isVineTouching && !parent.isWallTouching && !isAcidTerrain)
            {
                TeleportToTop(collision, 0.2f);
            }

            parent.Grounding();
            checkGround = true;
        }
        if (collision.transform.tag == "PlatformWood")
        {
            if (shouldTeleportToTop && !parent.isVineTouching && !parent.isWallTouching)
            {
                TeleportToTop(collision, 0.25f);
            }

            parent.Grounding();
            checkGround = true;
        }
        if (collision.transform.tag == "Platform")
        {
            parent.Grounding();
            parent.gameObject.transform.SetParent(collision.transform);
        }
        if (collision.transform.tag == "JumpingPlatform" && parent.enablePlatform)
        {
            parent.Grounding();
            float jumpForce = collision.transform.GetComponent<JumpingPlatform>().jumpForce;
            parent.JumpWithPlatform(jumpForce, true);
        }
        if (collision.transform.tag == "Teleporter" && parent.enablePlatform)
        {
            parent.Grounding();
            parent.gameObject.transform.SetParent(collision.transform);
        }
        if (collision.transform.tag == "Enemy")
        {
            parent.Grounding();
            //parent.GetDamage();
        }

        /*****************************************************************************************************************************/
        if (collision.transform.tag == "BreakablePlatform" && parent.enablePlatform)
        {
            parent.Grounding();
            collision.transform.GetComponent<BreakablePlatform>().startTimer = true;
        }
        /*****************************************************************************************************************************/
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "PlatformWood")
        {
            parent.Grounding();
            checkGround = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Platform" || collision.transform.tag == "Teleporter")
        {
            parent.gameObject.transform.SetParent(null);
        }

        // IMPORTANTE: al salir de cualquier superficie "pisable" hay que quitar el suelo.
        if (
            collision.transform.tag == "Ground"
            || collision.transform.tag == "PlatformWood"
            || collision.transform.tag == "Platform"
            || collision.transform.tag == "BreakablePlatform"
            || collision.transform.tag == "JumpingPlatform"
        )
        {
            parent.JumpWithPlatform(0, false);
            checkGround = false;
            parent.SetAtGround(false);
            shouldTeleportToTop = true;
        }
    }

    private void TeleportToTop(Collision2D collision, float amount)
    {
        if(teleportTimer >= teleportCooldown && !parent.isCrouching && !parent.isJumping && !parent.isFalling && !parent.isRoofTouching &&
           !parent.stopWalls)
        {
            Debug.Log("XD: 3: collisionando con: " + collision.gameObject.name);
            teleportTimer = 0f;
            parent.transform.position = new Vector3(parent.transform.position.x, collision.contacts[0].point.y + parent.GetComponent<BoxCollider2D>().size.y / 2 + amount, parent.transform.position.z);
            shouldTeleportToTop = false;
        }
    }
}
