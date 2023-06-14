using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ara;
public class Ground : MonoBehaviour
{
    public bool hasGem;
    public bool isFinal;
    public int index;

    [Header("Yellow Frame")]
    public float speedColor;
    public bool isYellowFrame;
    public SpriteRenderer spr;
    public AraTrail spotLight;
    public GameObject hole;
    public Transform targetJumpSimulation;
    public Collider collider;

    private void Awake()
    {
        if (isFinal)
        {
            hole = transform.parent.transform.GetChild(1).gameObject;
            return;
        }

        Color c = spr.color;
        c.a = 0;
        spr.color = c;

        spotLight.emit = true;

        SpotLight(false);
    }

    private void Update()
    {
        if (isFinal) return;

        if (isYellowFrame)
        {
            Color c = spr.color;
            if(c.a < 1)
            {
                c.a += Time.deltaTime * speedColor;
            }
            spr.color = c;
        }
        else
        {
            Color c = spr.color;
            if (c.a > 0)
            {
                c.a -= Time.deltaTime * speedColor;
            }
            spr.color = c;
        }
    }

    public void SpotLight(bool isActive)
    {
        if (spotLight != null)
            spotLight.enabled = isActive;
    }
}