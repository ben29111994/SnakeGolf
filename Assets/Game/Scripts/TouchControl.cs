using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody rig;
    private float m_DistanceY;
    private Plane m_Plane;
    private Vector3 m_DistanceFromCamera;
    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Vector3 changedPosition;
    private int frame_hold;

    void Start()
    {        
        m_DistanceY = Camera.main.transform.position.y;
        m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - m_DistanceY, Camera.main.transform.position.z);
        m_Plane = new Plane(Vector3.up, m_DistanceFromCamera);
    }

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {          
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float enter = 0.0f;

            if (m_Plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                currentPosition = hitPoint;
                changedPosition = currentPosition - lastPosition;
                lastPosition = currentPosition;

                if(frame_hold == 0)
                {
                    changedPosition = Vector3.zero;
                    frame_hold++;
                    return;
                }

                if(changedPosition.sqrMagnitude > 0)
                {
                 //   transform.position += changedPosition;
                    rig.velocity += moveSpeed * changedPosition * Time.deltaTime;
                }
            }
        }
        else
        {
            currentPosition = lastPosition = Vector3.zero;
            changedPosition = Vector3.Lerp(changedPosition, Vector3.zero, 1 * Time.deltaTime);
            frame_hold = 0;
        }
    }


}
