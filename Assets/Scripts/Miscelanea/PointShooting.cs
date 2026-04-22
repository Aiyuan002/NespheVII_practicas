using UnityEngine;

public class PointShooting : MonoBehaviour
{
    [Tooltip("12 clips: index 0=Horizontal, 1-10=Diagonal1-10, 11=Up")]
    public string[] rightClipNames;
    public string[] leftClipNames;
    public string[] legsClipNames;

    public Transform[] leftShootPoints;
    public Transform[] rightShootPoints;

    private Camera mainCamera;

    private readonly float[] targetAngles = {
        90f,   // Element 0: Up
        75f,   // Element 1: Diagonal 1
        60f,   // Element 2: Diagonal 2
        45f,   // Element 3: Diagonal 3
        30f,   // Element 4: Diagonal 4
        20f,   // Element 5: Diagonal 5
        -10f,  // Element 6: Diagonal 6 (¡HACIA ABAJO!)
        0f,    // Element 7: Horizontal
        -25f,  // Element 8: Diagonal 7
        -45f,  // Element 9: Diagonal 8
        -65f,  // Element 10: Diagonal 9
        -90f   // Element 11: Diagonal 10 (Abajo del todo)
    };

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void GetShootInfo(out string clipNameBody, out string clipNameLegs, out bool aimingRight, out int zoneIndex, out Transform shootPoint)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = ((Vector2)(mouseWorld - transform.position)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Debug.DrawRay(transform.position, direction * 2f, Color.red, 0.5f);
        aimingRight = angle > -90f && angle < 90f;

        float verticalAngle;
        if (aimingRight)
        {
            verticalAngle = angle;
        }
        else
        {
            if (angle > 0)
                verticalAngle = 180f - angle;
            else
                verticalAngle = -180f - angle;
        }


        zoneIndex = GetZoneIndex(verticalAngle);


        string[] clips = aimingRight ? rightClipNames : leftClipNames;
        clipNameBody = (clips != null && zoneIndex < clips.Length) ? clips[zoneIndex] : "";
        shootPoint = aimingRight ? rightShootPoints[zoneIndex] : leftShootPoints[zoneIndex];

        if(zoneIndex == 0)
        {
            clipNameLegs = legsClipNames[1]; // Up
        }
        else if(aimingRight)
        {
            clipNameLegs = legsClipNames[2];
        }
        else
        {
            clipNameLegs = legsClipNames[0];
        }


    }

    //private int GetZoneIndex(float verticalAngle)
    //{
    //    int result;

    //    if (verticalAngle < 90 && verticalAngle > 80)
    //    {
    //        result = 0;
    //    }
    //    else if (verticalAngle <= 80 && verticalAngle > 70)
    //    {
    //        result = 1;
    //    }
    //    else if (verticalAngle <= 70 && verticalAngle > 60)
    //    {
    //        result = 2;
    //    }
    //    else if (verticalAngle <= 60 && verticalAngle > 50)
    //    {
    //        result = 3;
    //    }
    //    else if (verticalAngle <= 50 && verticalAngle > 40)
    //    {
    //        result = 4;
    //    }
    //    else if (verticalAngle <= 40 && verticalAngle > 30)
    //    {
    //        result = 5;
    //    }
    //    else if (verticalAngle <= 30 && verticalAngle > 20)
    //    {
    //        result = 6;
    //    }
    //    else if (verticalAngle < 20 && verticalAngle > 0)
    //    {
    //        result = 7;
    //    }
    //    else if(verticalAngle < 0 && verticalAngle > -10)
    //    {
    //        result = 8;
    //    }
    //    else if (verticalAngle <= -10 && verticalAngle > -40)
    //    {
    //        result = 9;
    //    }
    //    else if (verticalAngle <= -40 && verticalAngle > -60)
    //    {
    //        result = 10;
    //    }
    //    else
    //    {
    //        result = 11; // Vertical Down
    //    }




    //    Debug.Log("Verical Angle: " + verticalAngle + " → Zone Index: " + result);
    //    return result;
    //}

    private int GetZoneIndex(float verticalAngle)
    {
        int closestIndex = 0;
        float minDifference = float.MaxValue;

        for (int i = 0; i < targetAngles.Length; i++)
        {
            // Calculamos qué tan lejos está el ángulo actual del "ideal" de la animación
            float diff = Mathf.Abs(verticalAngle - targetAngles[i]);

            if (diff < minDifference)
            {
                minDifference = diff;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}