using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPos : MonoBehaviour
{
    public bool isTrigger;
    public int indexTargetGround;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.isGroundFinal) return;

        if (other.CompareTag("GroundNormal"))
        {
            isTrigger = true;

            Ground ground = other.gameObject.GetComponent<Ground>();
            indexTargetGround = ground.index;
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GroundNormal"))
        {
            if (GameManager.Instance.isGroundFinal) return;

            isTrigger = false;

            Ground ground = other.gameObject.GetComponent<Ground>();
            indexTargetGround = ground.index;


            if (indexTargetGround == GameManager.Instance.targetObject.GetComponent<Ground>().index)
            {
                Debug.Log("STOP FORCE NOW");
                GameManager.Instance.player.trajectorySimulation.StopForceNow_2();
            }
        }      
    }
}
