using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public RectTransform topUI;
    public float top_X;
    public float top_Other;

    private void Awake()
    {
        SetFOV();
    }

    void SetFOV()
    {
        float ratio = cam.aspect;
        if (ratio >= 0.75) // 3:4
        {
            Camera.main.fieldOfView = 53;

            topUI.anchoredPosition = new Vector2(topUI.anchoredPosition.x, top_Other);
        }
        else
        if (ratio >= 0.56) // 9:16
        {
            Camera.main.fieldOfView = 53;

            topUI.anchoredPosition = new Vector2(topUI.anchoredPosition.x, top_Other);
        }
        else
        if (ratio >= 0.45) // 9:19
        {
            Camera.main.fieldOfView = 60;

            topUI.anchoredPosition = new Vector2(topUI.anchoredPosition.x, top_X);
        }
    }
}
