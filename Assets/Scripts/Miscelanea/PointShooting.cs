using UnityEngine;

public class PointShooting : MonoBehaviour
{
    [Tooltip("12 clips: index 0=Horizontal, 1-10=Diagonal1-10, 11=Up")]
    public string[] rightClipNames;
    public string[] leftClipNames;

    public Transform[] leftShootPoints;
    public Transform[] rightShootPoints;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void GetShootInfo(out string clipName, out bool aimingRight, out int zoneIndex, out Transform shootPoint)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = ((Vector2)(mouseWorld - transform.position)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Debug.DrawRay(transform.position, direction * 2f, Color.red, 0.5f);
        aimingRight = angle > -90f && angle < 90f;

        Debug.Log("Angle:" + angle + " → Aiming Right: " + aimingRight);
        float verticalAngle;
        if (aimingRight)
        {
            verticalAngle = Mathf.Clamp(angle, -90f, 90f);
        }
        else
        {
            verticalAngle = 180f - angle; // 180-180=0(horiz), 180-90=90(up)
        }


        zoneIndex = GetZoneIndex(verticalAngle);

        string[] clips = aimingRight ? rightClipNames : leftClipNames;
        Debug.Log("Selected Clip Index: " + zoneIndex + " → Clip Name: " + ((clips != null && zoneIndex < clips.Length) ? clips[zoneIndex] : "N/A"));
        clipName = (clips != null && zoneIndex < clips.Length) ? clips[zoneIndex] : "";
        shootPoint = aimingRight ? rightShootPoints[zoneIndex] : leftShootPoints[zoneIndex];
    }

    private int GetZoneIndex(float verticalAngle)
    {
        int result;

        if (verticalAngle < 90 && verticalAngle > 80)
        {
            result = 0;
        }
        else if (verticalAngle <= 80 && verticalAngle > 70)
        {
            result = 1;
        }
        else if (verticalAngle <= 70 && verticalAngle > 60)
        {
            result = 2;
        }
        else if (verticalAngle <= 60 && verticalAngle > 50)
        {
            result = 3;
        }
        else if (verticalAngle <= 50 && verticalAngle > 40)
        {
            result = 4;
        }
        else if (verticalAngle <= 40 && verticalAngle > 30)
        {
            result = 5;
        }
        else if (verticalAngle <= 30 && verticalAngle > 20)
        {
            result = 6;
        }
        else if (verticalAngle < 20 && verticalAngle > 0)
        {
            result = 7;
        }
        else if(verticalAngle < 0 && verticalAngle > -10)
        {
            result = 8;
        }
        else if (verticalAngle <= -10 && verticalAngle > -40)
        {
            result = 9;
        }
        else if (verticalAngle <= -40 && verticalAngle > -60)
        {
            result = 10;
        }
        else
        {
            result = 11; // Vertical Down
        }




        Debug.Log("Verical Angle: " + verticalAngle + " → Zone Index: " + result);
        return result;
    }
}