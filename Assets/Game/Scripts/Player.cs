using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ara;

public class Player : MonoBehaviour
{
    [Header("Scripts")]
    public TrajectorySimulation trajectorySimulation;
    public Ball ball;
    public AraTrail trail;

    public void SetDirection(Vector3 dir)
    {
        trajectorySimulation.transform.rotation = Quaternion.LookRotation(dir,Vector3.up);
    }

    public void SetActiveTrail(bool active)
    {
        trail.emit = active;
    }

    public void SetTimeTail(float n)
    {
        float time = 0.4f + n / 20.0f;
        trail.time = time;
    }

}
