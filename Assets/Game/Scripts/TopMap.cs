using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopMap : MonoBehaviour
{
    public GameObject[] elements;
    public GameObject mountain;

    private void OnEnable()
    {
        Vector3 pos = GameManager.Instance.listGrounds[GameManager.Instance.listGrounds.Count - 1].transform.position;
        pos.y = 0;
        pos.x += Random.Range(-20, 20);
        pos.z += Random.Range(0, 20);

        transform.position = pos;

        float angleY = Random.Range(0, 360);
        Vector3 angle = mountain.transform.eulerAngles;
        angle.y = angleY;

        mountain.transform.eulerAngles = angle;

        int r = Random.Range(0, elements.Length);

        if(r == 1)
        {
            int a = Random.Range(0, elements[1].transform.GetChild(0).childCount);

            for (int i = 0; i < elements[1].transform.GetChild(0).childCount; i++)
            {

                if(a == i)
                {
                    elements[1].transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    elements[1].transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }
           
        }

        for(int i = 0; i < elements.Length; i++)
        {
            if(i == r)
            {
                elements[i].SetActive(true);
            }
            else
            {
                elements[i].SetActive(false);
            }
        }

    }
}
