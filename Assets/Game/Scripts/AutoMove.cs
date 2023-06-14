using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    private int index;
    private Transform[] listPaths;

    int d;
    public Transform paths;
    public float moveSpeed;
    public float rotateSpeed;

    private void Start()
    {
        moveSpeed = Random.Range(200, 500) / 100;
        int r = Random.Range(0, 2);
        d = (r == 0) ? 1 : -1;

        listPaths = new Transform[paths.childCount];

        for(int i = 0; i < paths.childCount; i++)
        {
            listPaths[i] = paths.GetChild(i).transform;
        }

        index = Random.Range(0, listPaths.Length);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        Vector3 direction = listPaths[index].position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
        

        float distance = Vector3.Distance(transform.position, listPaths[index].position);

        if(distance < 4)
        {
            index += d;

            if(d == 1)
            {
                if (index >= listPaths.Length) index = 0;

            }
            else
            {
                if (index < 0) index = listPaths.Length-1;
            }
        }
    }
}
