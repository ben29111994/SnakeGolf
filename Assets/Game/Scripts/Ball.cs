using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using Ara;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidbody;
    private Renderer renderer;
    private int currentQueue;
    private Vector3 lastVelocity;
    private Vector3 lastAngular;
    public float drag;
    public float gravity;
    public float forceFinal;
    public bool isSetFinal;
    public bool isFly;
    public bool isGrounded;
    public int raycastIndex;
    public bool isContactFirstGround;
    public SnakeSimulation snakeSimulation;

    public Color[] colorPivots;
    public bool isDetectGroundFinal;
    public GameObject aceAnim;

    private void Awake()
    {
        MMVibrationManager.iOSInitializeHaptics();

        gravity = Physics.gravity.y;
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        currentQueue = renderer.material.renderQueue;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);


        if (Physics.SphereCast(ray, 0.25f, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag("pivot-finish-1"))
            {
                if (raycastIndex != 1)
                {
                    raycastIndex = 1;
                    GameManager.Instance.EffectPivotFinish(0);
                }
            }
            else if (hit.collider.gameObject.CompareTag("pivot-finish-2"))
            {
                if (raycastIndex != 2)
                {
                    raycastIndex = 2;
                    GameManager.Instance.EffectPivotFinish(1);
                }
            }
            else if (hit.collider.gameObject.CompareTag("pivot-finish-3"))
            {
                if (raycastIndex != 3)
                {
                    raycastIndex = 3;
                    GameManager.Instance.EffectPivotFinish(2);
                }
            }
            else if (hit.collider.gameObject.CompareTag("GroundFinal"))
            {
                if (raycastIndex != 4)
                {
                    raycastIndex = 4;
                    GameManager.Instance.EffectPivotFinish(1);
                }
            }
            else
            {
                if (raycastIndex != 10)
                {
                    raycastIndex = 10;
                    GameManager.Instance.EffectPivotFinish(10);
                }        
            }
        }
        else
        {
            if (raycastIndex != 10)
            {
                raycastIndex = 10;
                GameManager.Instance.EffectPivotFinish(10);
            }
        }
    }

    private float tVibration;
    private void Update()
    {
        if(tVibration < 0.1f)
        {
            tVibration += Time.deltaTime;
        }
        
    }

    public void RefreshGravity()
    {
        isDetectGroundFinal = false;
        Physics.gravity = Vector3.up * gravity;
    }


    public bool isAnimation;

    private void LateUpdate()
    {
        lastAngular = rigidbody.angularVelocity;

        snakeSimulation.UpdateStep(isAnimation);
    }

    private IEnumerator C_WailForBodyPath(GameObject collision)
    {
        Ground ground = collision.GetComponent<Ground>();
        int groundIndex = ground.index;

        bool isWhile = true;

        while (isWhile)
        {
            if (GameManager.Instance.isFever)
            {
                GameManager.Instance.UpdateTurn(groundIndex);
                yield break;
            }


            int count = snakeSimulation.listPaths.Count;
            int n = 0;

            for (int i = 1; i < count; i++)
            {
                if (snakeSimulation.listPaths[0].bodyPath.position == snakeSimulation.listPaths[i].bodyPath.position)
                {
                    n++;
                }
            }

            if (n == count - 1)
            {
                isWhile = false;
            }


            yield return null;
        }

        if (GameManager.Instance.player.trajectorySimulation.isForce) yield break;

        yield return new WaitForSeconds(0.0f);

        isAnimation = false;

        GameManager.Instance.UpdateTurn(groundIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundNormal"))
        {
            if (isContactFirstGround == false)
            {
                isContactFirstGround = true;
            }
 

            Vector3 ballPosition = transform.position;
            ballPosition.y = 0.0f;

            Vector3 collisionPosition = collision.gameObject.transform.position;
            collisionPosition.y = 0.0f;

            float distancetoTarget = Vector3.Distance(ballPosition, collisionPosition);

            if(distancetoTarget > 2.65f)
            {
                GameObject nearMiss = PoolManager.instance.GetObject(PoolManager.NameObject.NearMiss);
                nearMiss.transform.position = transform.position;
                nearMiss.SetActive(true);
                // near miss
            }

            rigidbody.drag = drag * 20;
            rigidbody.velocity = Vector3.zero;
            isGrounded = true;
            GameManager.Instance.EffectBodyPath(gameObject);

            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.SphereCast(ray, 0.25f, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag("pivot-1"))
                {
                    if (GameManager.Instance.feverNumber > 0 && GameManager.Instance.index < GameManager.Instance.maxIndex - 2)
                    {
                        GameManager.Instance.isFever = true;
                    }


                    HideColliderPivot(hit.collider.gameObject.transform);
                    GameManager.Instance.Effect_ElipseDual(hit.collider.transform.position);
                    GameManager.Instance.ScorePlus(hit.collider.transform.position, 3, "PERFECT",colorPivots[2]);

                    snakeSimulation.GenerateBodyPath(1);
                    GameManager.Instance.player.SetTimeTail(snakeSimulation.listPaths.Count);
                }
                else if (hit.collider.gameObject.CompareTag("pivot-2"))
                {
                    HideColliderPivot(hit.collider.gameObject.transform);
                    GameManager.Instance.Effect_ElipseDual(hit.collider.transform.position);
                    GameManager.Instance.ScorePlus(hit.collider.transform.position, 2, "AMAZING",colorPivots[1]);
                    snakeSimulation.GenerateBodyPath(1);
                    GameManager.Instance.player.SetTimeTail(snakeSimulation.listPaths.Count);
                }
                else
                {
                    if (GameManager.Instance.index >= 1)
                        GameManager.Instance.ScorePlus(hit.collider.transform.position, 1, "GOOD", colorPivots[0]);
                }

            }

            StartCoroutine(C_WailForBodyPath(collision.gameObject));
        }
        else if (collision.gameObject.CompareTag("GroundFinal"))
        {
            if (isSetFinal) return;

            isSetFinal = true;
            isDetectGroundFinal = true;
            Physics.gravity = Vector3.up * -9.81f * 1.5f;
            rigidbody.drag = drag;
            rigidbody.angularDrag = 0.4f;
            rigidbody.AddForce(Vector3.forward * forceFinal);
            GameManager.Instance.CheckFail();
        }
        else if (collision.gameObject.CompareTag("Hole"))
        {
            Vector3 playerPos = GameManager.Instance.player.ball.gameObject.transform.position;
            playerPos.y = 1f;
            ScorePlusComplete(playerPos,50,5);


            collision.gameObject.SetActive(false);
            Physics.gravity = Vector3.up * -9.81f * 15f;

            rigidbody.velocity = lastVelocity;
            rigidbody.angularVelocity = lastAngular;
            rigidbody.drag = drag;
            ChangeLayerToNone();

            if(isDetectGroundFinal == false)
            {
                ScorePlusComplete(playerPos, 1, 10);
                aceAnim.SetActive(true);
            }
        }
        else if (collision.gameObject.CompareTag("Plane"))
        {
            GameManager.Instance.plane.enabled = false;

            rigidbody.velocity = lastVelocity;
            rigidbody.angularVelocity = lastAngular;
            rigidbody.drag = drag;
            StartCoroutine(C_ContactPlane());
        }
    }

    IEnumerator C_ContactPlane()
    {
        yield return new WaitForSeconds(0.25f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        GameManager.Instance.CheckFail();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BotHole"))
        {
            GameManager.Instance.HoleTextAnim("HOLE IN ONE");
            GameManager.Instance.ScoreFinish(50);
            GameManager.Instance.Complete();
        }
        else if (other.CompareTag("gem"))
        {
            GameManager.Instance.EffectGem(other.gameObject);
        }else if (other.CompareTag("bodypath"))
        {
            if (isGrounded && GameManager.Instance.isFirstTurn && GameManager.Instance.isFever == false)
            {
                GameManager.Instance.EffectBodyPath(gameObject);

                Vibration();
                ScorePlusBodyPath(GameManager.Instance.player.ball.gameObject.transform.position, 1);
            }
        }
        //}else if (other.CompareTag("pivot-1"))
        //{
        //    if(GameManager.Instance.feverNumber > 0)
        //        GameManager.Instance.isFever = true;

        //    HideColliderPivot(other.gameObject.transform);
        //    GameManager.Instance.Effect_ElipseDual(other.transform.position);
        //    GameManager.Instance.ScorePlus(other.transform.position, 3,"PERFECT");

        //    snakeSimulation.GenerateBodyPath(3);
        //    GameManager.Instance.player.SetTimeTail(snakeSimulation.listPaths.Count);
        //}
        //else if (other.CompareTag("pivot-2"))
        //{
        //    HideColliderPivot(other.gameObject.transform);
        //    GameManager.Instance.Effect_ElipseDual(other.transform.position);
        //    GameManager.Instance.ScorePlus(other.transform.position ,2,"GOOD");

        //    snakeSimulation.GenerateBodyPath(2);
        //    GameManager.Instance.player.SetTimeTail(snakeSimulation.listPaths.Count);
        //}

    }

    private void ChangeLayerToNone()
    {
        int count = snakeSimulation.listPaths.Count;

        for(int i = 0; i < count; i++)
        {
            snakeSimulation.listPaths[i].bodyPath.GetComponent<Renderer>().material.renderQueue = currentQueue - 2;
        }
        
        renderer.material.renderQueue = currentQueue-2;
        gameObject.layer = 11;
    }

    public void ResetLayer()
    {
        renderer.material.renderQueue = currentQueue;
        gameObject.layer = 13;
    }

    private void HideColliderPivot(Transform trans)
    {
        int count = trans.parent.transform.childCount;

        for(int i = 0; i < count; i++)
        {
            Collider collider = trans.parent.transform.GetChild(i).gameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    public void ScorePlusBodyPath(Vector3 position, int score)
    {
        StartCoroutine(C_ScorePlusBodyPath(position, score));
    }

    private IEnumerator C_ScorePlusBodyPath(Vector3 position, int score)
    {
        for (int i = 0; i < score; i += 2)
        {
            GameObject scoreObject = PoolManager.instance.GetObject(PoolManager.NameObject.ScorePlusBodyPath);

            int s = (score <= 1) ? 1 : 2;
            scoreObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "+" + s;

            Vector3 tarPos = position;
            tarPos.x += Random.Range(-25, 25) / 10.0f;
            tarPos.z += Random.Range(-20, 20) / 10.0f;

            scoreObject.transform.position = tarPos;
            scoreObject.SetActive(true);

            GameManager.Instance.PlusGameScore(s);
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void ScorePlusComplete(Vector3 position,int times, int score)
    {
        StartCoroutine(C_ScorePlusComplete(position,times, score));
    }

    private IEnumerator C_ScorePlusComplete(Vector3 position, int times,int s)
    {
        for (int i = 0; i < times; i += 2)
        {
            Vibration();

            GameObject scoreObject = PoolManager.instance.GetObject(PoolManager.NameObject.ScorePlusBodyPath);

            scoreObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "+" + s;

            Vector3 tarPos = position;
            tarPos.x += Random.Range(-25, 25) / 10.0f;
            tarPos.z += Random.Range(-20, 20) / 10.0f;

            scoreObject.transform.position = tarPos;
            scoreObject.SetActive(true);

            GameManager.Instance.PlusGameScore(s);
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void Vibration()
    {
        if (tVibration >= 0.1f)
        {
            if(GameManager.Instance.isVibration)
              MMVibrationManager.iOSTriggerHaptics(HapticTypes.MediumImpact);
        }
    }

    public void VibrationShoot()
    {
        if (GameManager.Instance.isVibration)
            MMVibrationManager.iOSTriggerHaptics(HapticTypes.HeavyImpact);
    }

    public AraTrail araTrail;
    public Gradient feverColor;
    public Gradient normalColor;


    public void ChangeColorTrailFever()
    {
        araTrail.colorOverLenght = feverColor;   
    }

    public void ChangeColorTrailNormal()
    {
        araTrail.colorOverLenght = normalColor;
    }
}
