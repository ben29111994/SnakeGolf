using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElipseD : MonoBehaviour
{
    public Renderer renderer;
    public AnimationCurve cur;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }


    float t;

    private void OnEnable()
    {
        t = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(t > 0)
        {
            t -= Time.deltaTime ;


            Color c = renderer.material.color;
            c.a = cur.Evaluate(t);
            renderer.material.color = c;
        }
    }
}
