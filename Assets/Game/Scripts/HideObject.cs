using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{
    public float time;

    private void OnEnable()
    {
        StartCoroutine(C_Hide());
    }

    private IEnumerator C_Hide()
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
 
}
