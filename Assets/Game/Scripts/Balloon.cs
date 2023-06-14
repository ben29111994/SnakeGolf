using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        Animator animator = GetComponent<Animator>();
        animator.enabled = false;

        float timeDelay = Random.Range(100, 1500) / 1000;

        yield return new WaitForSeconds(timeDelay);

        animator.enabled = true;
    }
}
