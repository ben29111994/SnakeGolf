using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
    private void OnEnable()
    {
        int angle = Random.Range(0, 360);

        Vector3 euler = transform.eulerAngles;
        euler.y = angle;
        transform.eulerAngles = euler;
    }
}
