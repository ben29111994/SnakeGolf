using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectorySimulation : MonoBehaviour
{
    [Header("Golf Club")]
    public GameObject pivotGolfClub;
    public GameObject directionGolfClub;
    public GameObject positionClubObject;
    public GameObject golfClub;
    public float minAgnleGolfClub;
    public float maxAngleGolfClub;

    [Header("Trajectory Simulation")]
    public bool isReflect;
    public float fireStrength;
    private float lastFireStrength;
    private Vector3 lastGravity;
    private Vector3 direction;
    public GameObject directionObject;
    public GameObject endPosition;

    public LineRenderer sightLine;
    public LayerMask layer;
    private const int segmentCount = 750;
    private const float segmentScale = .1f;
    private Collider _hitObject;
    private Collider hitObject { get { return _hitObject; } }

    [Header("CameraAnimation")]
    public Camera cam;
    private float currentFOV;
    private float targetFOV;
    public float distanceFOV;

    [Header("Force Manager")]
    public bool isLoop;
    public bool isForce;

    public float minAngle;
    public float maxAngle;

    public float minForce;
    public float maxForce;

    public float speed;
    public float t;
    public int dir;

    private float minA, maxA, minF, maxF, maxS;

    private bool isAutoForce;

    [Header("Jump Simulation")]
    public PhysicJumpSimulation physicJumpSimulation;

    private void Start()
    {
        currentFOV = cam.fieldOfView;
        targetFOV = currentFOV + distanceFOV;
    }

    void FixedUpdate()
    {
        if (isForce)
        {
            if (isLoop)
            {
                t += (Time.deltaTime * dir * maxS) / Time.timeScale;

                if (t > 1)
                {
                    dir *= -1;
                    t = 1;
                }
                else if (t < 0)
                {
                    dir *= -1;
                    t = 0;
                }

                fireStrength = Mathf.Lerp(minF, maxF, t);

                Vector3 temp = transform.eulerAngles;
                temp.x = Mathf.Lerp(minA, maxA, t);
                transform.eulerAngles = temp;
            }
            else
            {
                t += Time.deltaTime * 1 * maxS;

                fireStrength = Mathf.Lerp(minF, maxF, t);

                Vector3 temp = transform.eulerAngles;
                temp.x = Mathf.Lerp(minA, maxA, t);
                transform.eulerAngles = temp;

                Vector3 posGround = targetGround.transform.position;
                posGround.y = Mathf.Lerp(currentPosY_Ground, targetPosY_Ground, t * 2);
                targetGround.transform.position = posGround;

                Vector3 posPlayer = GameManager.Instance.player.transform.position;
                posPlayer.y = Mathf.Lerp(currentPosY_Player, targetPosY_Player, t * 2);
                GameManager.Instance.player.transform.position = posPlayer;

                Vector3 eulerAngleGolfClub = golfClub.transform.eulerAngles;
                eulerAngleGolfClub.x = Mathf.Lerp(minAgnleGolfClub, maxAngleGolfClub, t);
                golfClub.transform.eulerAngles = eulerAngleGolfClub;

                cam.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, t);

                if (t >= 1)
                {
                    StopForceNow();
                }
            }

        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentFOV, 2 * Time.deltaTime);
        }

        // if change transform or fireStrength , update trajectory

        if (isForce == false) return;

        if (transform.hasChanged || lastFireStrength != fireStrength || lastGravity != Physics.gravity)
        {
            DrawTrajectoryPath();

            lastGravity = Physics.gravity;
            lastFireStrength = fireStrength;
            transform.hasChanged = false;
        }
    }

    private IEnumerator C_AnimGround()
    {
        Vector3 fPos = targetGround.transform.position;
        Vector3 tPos = fPos;
        tPos.y = currentPosY_Ground;

        Vector3 fScale = pivotGolfClub.transform.localScale;
        Vector3 tScale = Vector3.zero;


        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;

            targetGround.transform.position = Vector3.Lerp(fPos, tPos, t);

            yield return null;
        }

    }

    private IEnumerator C_AnimGolfClub()
    {

        Vector3 fAngle = golfClub.transform.eulerAngles;
        Vector3 tAngle = golfClub.transform.eulerAngles;
        tAngle.x = -0.0f;


        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 20;

            golfClub.transform.eulerAngles = Vector3.Lerp(fAngle, tAngle, t);
            yield return null;
        }
        GameManager.Instance.player.ball.snakeSimulation.SetDistance();
        GameManager.Instance.player.ball.VibrationShoot();
        GameManager.Instance.currentObject.GetComponent<Ground>().isYellowFrame = false;
        GameManager.Instance.player.ball.isGrounded = false;
        GameManager.Instance.player.ball.isFly = true;
        GameManager.Instance.player.ball.isAnimation = true;

        ball.drag = 0.0f;
        ball.AddForce(direction * fireStrength);

        StartCoroutine(C_AnimGround());
    
        golfClub.SetActive(false);
        endPosition.SetActive(false);
        PoolManager.instance.RefreshDot();

        if (GameManager.Instance.isFirstTurn == false)
            GameManager.Instance.isFirstTurn = true;
    }

    private void GolfClubContactBall_Fly()
    {

    }

    private void Update()
    {
        if (isRotateDirection == false) return;

        RotateDirection();
    }

    [Header("RotateControl")]
    public bool isRotateDirection;
    public float speed_rotate;
    public float distanceAngleRotate;
    public float angleYRotate;
    private float minAngle_Rotate;
    private float maxAngle_Rotate;
    private float t_rotate;
    private int dir_rotate;
    public GameObject arrow;

    [Header("Ground")]
    private GameObject targetGround;
    public float push_Force;
    private float currentPosY_Ground;
    private float targetPosY_Ground;

    private float currentPosY_Player;
    private float targetPosY_Player;

    public void StartRotateDirection()
    {
        isRotateDirection = true;
        t_rotate = 0;
        dir_rotate = 1;
        angleYRotate = transform.eulerAngles.y;
        minAngle_Rotate = angleYRotate - distanceAngleRotate;
        maxAngle_Rotate = angleYRotate + distanceAngleRotate;
        arrow.SetActive(true);
    }

    public void StopRotateDirection()
    {
        isRotateDirection = false;
        arrow.SetActive(false);
    }

    private void RotateDirection()
    {
        t_rotate += Time.deltaTime * dir_rotate * speed_rotate;

        if (t_rotate > 1)
        {
            t_rotate = 1;
            dir_rotate = -1;
        } else if (t_rotate < 0)
        {
            t_rotate = 0;
            dir_rotate = 1;
        }

        angleYRotate = Mathf.Lerp(minAngle_Rotate, maxAngle_Rotate, t_rotate);

        Vector3 temp = transform.eulerAngles;
        temp.y = angleYRotate;
        transform.eulerAngles = temp;
        //  arrow.transform.eulerAngles = temp;
    }

    public void StartForce()
    {
        GameManager.Instance.currentObject.GetComponent<Ground>().isYellowFrame = true;

        isForce = true;
        isAutoForce = false;

        dir = 1;
        t = 0;

        if (GameManager.Instance.isGroundFinal)
        {
            minA = minAngle;
            maxA = maxAngle;

            minF = minForce;
            maxF = maxForce;

            maxS = speed;
        }
        else
        {
            minA = minAngle;
            maxA = maxAngle;

            minF = minForce;
            maxF = maxForce;

            maxS = speed;
        }
        targetGround = GameManager.Instance.currentObject;
        currentPosY_Ground = targetGround.transform.position.y;
        targetPosY_Ground = currentPosY_Ground - push_Force;

        currentPosY_Player = GameManager.Instance.player.transform.position.y;
        targetPosY_Player = currentPosY_Player - push_Force;
    }

    public void StopForce()
    {
        if (isAutoForce || isForce == false) return;

        arrow.SetActive(false);

        isForce = false;
       // endPosition.SetActive(false);
    //    PoolManager.instance.RefreshDot();
        Shoot();
    }

    public void StopForceNow()
    {
        StopForce();
        isAutoForce = true;
    }

    public void StopForceNow_2()
    {
        if (isStopForceNow_2) return;

        isStopForceNow_2 = true;
        StartCoroutine(C_StopForceNow_2());
    }

    private bool isStopForceNow_2;

    private IEnumerator C_StopForceNow_2()
    {
        yield return new WaitForSeconds(0.05f);
        isStopForceNow_2 = false;
        StopForce();
        isAutoForce = true;
    }

    public void Refresh()
    {
        isAutoForce = false;
        isRotateDirection = false;
        arrow.SetActive(false);
    }

    public Vector3[] segments;

    private void DrawTrajectoryPath()
    {
        segments = new Vector3[segmentCount];

        segments[0] = transform.position;

        direction = (directionObject.transform.position - transform.position).normalized;
        Vector3 segVelocity = direction * fireStrength * Time.deltaTime;

        _hitObject = null;

        int n = 0;

        for (int i = 1; i < segmentCount; i++)
        {

            float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;

            segVelocity = segVelocity + Physics.gravity * segTime;

            RaycastHit hit;
            if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale, layer))
            {
                if (isReflect)
                {
                    _hitObject = hit.collider;
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                    segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
                    segVelocity = Vector3.Reflect(segVelocity, hit.normal);
                }
                else
                {
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                    i = segmentCount;
                }
            }
            else
            {
                segments[i] = segments[i - 1] + segVelocity * segTime;
            }
        }

        // LineRenderer
        int count = 0;
        for (int i = 0; i < segmentCount; i++)
        {
            if (segments[i] != Vector3.zero)
            {
                count++;
            }
            else
            {
                i = segmentCount;
            }
        }

        //   sightLine.SetVertexCount(count);

        for (int i = 0; i < count; i++)
        {
            // sightLine.SetPosition(i, segments[i]);
        }

        PoolManager.instance.RefreshDot();

        GameObject dotObject = null;

        for (int i = 0; i < count; i += 7)
        {
            dotObject = PoolManager.instance.GetObject(PoolManager.NameObject.Dot);
            dotObject.transform.position = segments[i];
            dotObject.SetActive(true);
        }

        dotObject.SetActive(false);

        endPosition.transform.position = segments[count - 1] + new Vector3(0,0.25f,0);

        Vector3 frPos = endPosition.transform.position;
        frPos.y = 0;
        Vector3 toPos = segments[0];
        toPos.y = 0;

        Vector3 dirEndpos = frPos - toPos;
        endPosition.transform.rotation = Quaternion.LookRotation(dirEndpos, Vector3.up);

        if (endPosition.activeSelf == false)
            endPosition.SetActive(true);

    }

    private void LateUpdate()
    {
        transform.position = ball.gameObject.transform.position;

        arrow.transform.position = transform.position;

        Vector3 rot = arrow.transform.eulerAngles;
        rot.y = transform.eulerAngles.y;
        arrow.transform.eulerAngles = rot;

        Vector3 eulerAnglePositionClubObject = positionClubObject.transform.eulerAngles;
        eulerAnglePositionClubObject.y = directionGolfClub.transform.eulerAngles.y;
        positionClubObject.transform.eulerAngles = eulerAnglePositionClubObject;

        
    }

    public Rigidbody ball;
    public void Shoot()
    {
         StartCoroutine(C_AnimGolfClub());
    }


    public void JumpSimulation()
    {
        GameManager.Instance.player.ball.isAnimation = true;
        GameManager.Instance.player.ball.snakeSimulation.SetDistance();

        GameManager.Instance.player.ball.rigidbody.drag = 0f;
        GameManager.Instance.player.ball.rigidbody.angularDrag = 0f;

        Transform tar = GameManager.Instance.targetObject.GetComponent<Ground>().targetJumpSimulation;

        if (tar == null) return;

        float angle = (float)Random.Range(700, 700) / 10.0f;

        physicJumpSimulation.JumpSimulation(GameManager.Instance.player.ball.rigidbody, tar, angle);
    }


    public void ActiveGolfClub()
    {
        golfClub.SetActive(true);
        pivotGolfClub.transform.localScale = Vector3.one;
        positionClubObject.transform.position = GameManager.Instance.player.ball.transform.position;

        Vector3 scale = positionClubObject.transform.localScale;
        scale.x = (GameManager.Instance.targetObject.transform.position.x - GameManager.Instance.currentObject.transform.position.x > 0) ? 1 : -1;
        positionClubObject.transform.localScale = scale;

        Vector3 eulerAngle = golfClub.transform.eulerAngles;
        eulerAngle.x = minAgnleGolfClub;
        golfClub.transform.eulerAngles = eulerAngle;
    }

   
}

