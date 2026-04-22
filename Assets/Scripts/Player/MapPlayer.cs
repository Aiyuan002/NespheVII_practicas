using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPlayer : MonoBehaviour
{
    public GameObject personaje;
    public RawImage rawImage;
    public Camera _camera;
    public Canvas canvas;
    public float YOffset;
    public float XOffset;
    public float Multiply;


    void Update()
    {

        Vector3 screenPos = personaje.transform.position * Multiply;

        Vector2 localPoint;
        RectTransform canvasRect = canvas.transform as RectTransform;

        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                null,
                out localPoint
            )
        )
        {
            localPoint += new Vector2(XOffset, YOffset);
            rawImage.rectTransform.anchoredPosition = localPoint;
        }
    }
}
