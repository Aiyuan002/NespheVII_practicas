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
    public float YOffset = 604.5f;
    public float XOffset = 277.8f;
    public float Multiply = 49.6f;


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
