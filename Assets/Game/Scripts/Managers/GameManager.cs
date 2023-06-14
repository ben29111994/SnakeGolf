using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (isResetData) PlayerPrefs.DeleteAll();


        Application.targetFrameRate = 60;
        Instance = (Instance == null) ? this : Instance;

        if(PlayerPrefs.GetInt("firstPlay") == 0)
        {
            PlayerPrefs.SetInt("firstPlay", 1);
            PlayerPrefs.SetInt("bodyPaths", 5);
        }
    }

    [Header("Game")]
    public int levelGame;
    public bool isPickLevel;
    public bool isAnalytic;
    public bool isVibration;
    public bool isResetData;
    public bool isFocusRotateCamera;

    [Header("Status")]
    public bool isCanShoot;
    public bool isShoot;
    public bool isFinish;
    public bool isGroundFinal;
    public bool isCheckFail;
    public bool isFever;
    public bool isFirstTurn;
    private bool isChangeColor;
    private bool isChangeBg;

    public int feverMaxNumber;
    public int feverNumber;
    public int index;
    public int maxIndex;
    public int gameScore;
    public int bodyPathsAmount;

    [Header("GameObject")]
    public Player player;
    public Collider plane;
    public GameObject topMap;
    public GameObject skullLand;
    public GameObject iceLand;

    [Header("Level")]
    public GameObject[] levels;
    private int lvl;
    private GameObject levelObject;
    [HideInInspector] public GameObject currentObject;
    [HideInInspector] public GameObject targetObject;
    public List<GameObject> listGrounds = new List<GameObject>();
    private GameObject effectRippleFinish;
    public GameObject[] listElementBgs;
    private GameObject elementBg;

    [Header("Camera")]
    private Vector3 dirFocusRotateCamera;
    private float posX_Camera;
    private float currentPosY_Camera;
    private float currentPosZ_Camera;
    private float currentRotX_Camera;
    public GameObject pivotCamera;
    public float speedLerpCamera;

    [Header("UI")]
    public Animator scoreAnim;
    public Text gameScoreText;
    public GameObject touchObject;
    public GameObject holeInOne;
    public Image levelBarImage;
    public Text fromLevel;
    public Text toLevel;
    public Text scoreCompletedText;

    [Header("Color Ground")]
    public List<Renderer> pivotFinishs = new List<Renderer>();
    public Color[] pivot_finish_color;
    public Color contact_finish_color;

    private void Start()
    {
        Init();
        GenerateLevel();
    }

    private void FixedUpdate()
    {
        if (isFinish) return;

        CameraControl(true);
    }

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            player.trajectorySimulation.JumpSimulation();
        }
    }

    private void StartEvent_Analytic()
    {
        if (isAnalytic == false) return;


    }

    private void EndEvent_Analytic()
    {
        if (isAnalytic == false) return;

    }

    private void EndLevelEvent_Analytic()
    {
        if (isAnalytic == false) return;

    }

    private IEnumerator C_Analytics_Start()
    {
        yield return new WaitForSeconds(2.0f);
        AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.StartEvent);
    }

    private IEnumerator C_Analytics_End()
    {
        yield return new WaitForSeconds(2.0f);
        AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.EndEvent);
    }

    private void Init()
    {
        levelGame = (isPickLevel) ? levelGame : PlayerPrefs.GetInt("levelGame");
        lvl = (levelGame >= levels.Length) ? Random.Range(levels.Length/2, levels.Length) : levelGame;

        fromLevel.text = (levelGame + 1).ToString();
        toLevel.text = (levelGame + 2).ToString();

        currentPosY_Camera = Camera.main.transform.localPosition.y;
        currentPosZ_Camera = Camera.main.transform.localPosition.z;
        currentRotX_Camera = Camera.main.transform.localEulerAngles.x;
    }

    private void Refresh()
    {
        Invoke("StartEvent_Analytic", 2);

        if (isChangeColor)
        {
            ColorManager.instance.ChangeColor();
            isChangeColor = false;
        }
        player.ball.snakeSimulation.ChangeSetColorBody();
        player.trajectorySimulation.endPosition.SetActive(false);
        player.ball.isContactFirstGround = false;
        levelBarImage.fillAmount = 0;
        maxIndex = 0;
        feverNumber = 0;
        player.SetTimeTail(0);
        isFever = false;
        player.ball.snakeSimulation.RefreshBodyPath();
        PoolManager.instance.RefreshGem();
        PoolManager.instance.RefreshAnimals();
        pivotFinishs.Clear();
        gameScore = PlayerPrefs.GetInt("scoreGame");
        gameScoreText.text = gameScore.ToString();
        listGrounds.Clear();
        index = 0;
        isGroundFinal = false;
        isFinish = false;
        isFirstTurn = false;
        plane.enabled = true;
        player.trajectorySimulation.Refresh();
        topMap.SetActive(false);

        fromLevel.text = (levelGame + 1).ToString();
        toLevel.text = (levelGame + 2).ToString();
        SetFeverNumber();
    }

    private void SetFeverNumber()
    {
        if(levelGame < 8)
        {
            feverMaxNumber = 2;
        }
        else if(levelGame < 25)
        {
            feverMaxNumber = 3;
        }
        else
        {
            feverMaxNumber = 3;
        }
    }

    public void GenerateLevel()
    {
        Refresh();

        float a = lvl % 2;
        Debug.Log(a);
        if(a == 0)
        {
            skullLand.SetActive(true);
            iceLand.SetActive(false);
        }
        else
        {
            skullLand.SetActive(false);
            iceLand.SetActive(true);
        }

        if (elementBg != null) Destroy(elementBg);

        elementBg = Instantiate(listElementBgs[lvl]) as GameObject;

        if (levelObject != null) Destroy(levelObject);

        levelObject = Instantiate(levels[lvl]) as GameObject;
        listGrounds.Clear();

        for (int i = 0; i < levelObject.transform.childCount; i++)
        {
            maxIndex++;
            listGrounds.Add(levelObject.transform.GetChild(i).gameObject);
            if (i < levelObject.transform.childCount - 1)
            {
                listGrounds[i].GetComponent<Ground>().index = i;
            }
            else
            {
                listGrounds[i].transform.GetChild(0).GetComponent<Ground>().index = i;
            }
        }

        listGrounds[0].transform.GetChild(0).gameObject.SetActive(false);

        for (int i = 0; i < listGrounds[listGrounds.Count - 1].transform.GetChild(1).GetChild(5).childCount; i++)
        {
            pivotFinishs.Add(listGrounds[listGrounds.Count - 1].transform.GetChild(1).GetChild(5).GetChild(i).GetComponent<Renderer>());
        }

        pivot_finish_color = new Color[pivotFinishs.Count];
        for (int i = 0; i < pivot_finish_color.Length; i++)
        {
            pivot_finish_color[i] = pivotFinishs[i].material.color;
        }

        effectRippleFinish = listGrounds[listGrounds.Count - 1].transform.GetChild(1).GetChild(6).gameObject;

        int middleNumber = listGrounds.Count / 2;

        // spawn animal
        for(int i = 0; i < listGrounds.Count;i += 4)
        {
            if(i < listGrounds.Count - 1)
            {
                GameObject balloonf = PoolManager.instance.GetObject(PoolManager.NameObject.Balloon);
                Vector3 posf = listGrounds[i].transform.position;
                posf.y = balloonf.transform.position.y;
                balloonf.transform.position = posf;
                balloonf.SetActive(true);
            }
        }

        GameObject balloon = PoolManager.instance.GetObject(PoolManager.NameObject.Balloon);
        Vector3 pos = listGrounds[listGrounds.Count-1].transform.position;
        pos.y = balloon.transform.position.y;
        balloon.transform.position = pos;
        balloon.SetActive(true);


        Vector3 posPlayer = listGrounds[index].transform.position;
        posPlayer.y = player.transform.localScale.x / 2.0f + 3.0f;
        player.transform.position = posPlayer;
        player.ball.transform.localPosition = Vector3.zero;
        player.ball.rigidbody.velocity = Vector3.zero;
        player.ball.rigidbody.angularVelocity = Vector3.zero;
        player.ball.rigidbody.drag = 0;
        player.ball.isSetFinal = false;
        player.ball.rigidbody.useGravity = true;

        player.ball.RefreshGravity();
        player.ball.ResetLayer();
        player.SetActiveTrail(true);

        topMap.SetActive(true);
        RefreshTransformCameraMain();
        StartCoroutine(C_Analytics_Start());
    }

    private void SpawnGem()
    {
        int numberGems = Random.Range(1, 4);

        for (int i = 0; i < numberGems; i++)
        {
            int r = Random.Range(1, listGrounds.Count);
            Ground ground = listGrounds[r].gameObject.GetComponent<Ground>();

            if (ground == null)
            {
                ground = listGrounds[r].transform.GetChild(0).GetComponent<Ground>();

                if (ground.hasGem)
                {
                    i--;
                }
                else
                {
                    Debug.Log(ground.name);
                    ground.hasGem = true;
                    Vector3 pos = ground.hole.transform.position;
                    pos.y = 1.5f;
                    pos.x += Random.Range(-1.5f, 1.5f);
                    pos.z += Random.Range(-1.5f, 1.5f);
                    SpawnGem(pos);
                }
            }
            else
            {
                if (ground.hasGem)
                {
                    i--;
                }
                else
                {
                    ground.hasGem = true;
                    Vector3 pos = ground.transform.position;
                    pos.y = 1.5f;
                    SpawnGem(pos);
                }
            }
        }
    }

    private void UpdateGround()
    {
        if (index >= listGrounds.Count) return;

        currentObject = listGrounds[index];
        targetObject = listGrounds[index + 1];

        GameManager.Instance.HideSpotLight();


        if (targetObject.CompareTag("GroundFinal")) return;


        Ground targetGround = targetObject.GetComponent<Ground>();
        targetGround.SpotLight(true);
    }

    public void HideSpotLight()
    {
        //currentObject.GetComponent<Ground>().isYellowFrame = false;

        for (int i = 0; i < listGrounds.Count - 1; i++)
        {
            listGrounds[i].GetComponent<Ground>().SpotLight(false);
        }
    }


    private void SetDirectionPlayer()
    {
        Vector3 playerPos = player.ball.transform.position;

        Vector3 targetPos = (targetObject.CompareTag("GroundFinal")) ? targetObject.transform.GetChild(1).gameObject.transform.position : targetObject.transform.position;
        playerPos.y = targetPos.y = 0.0f;
        Vector3 dir = targetPos - playerPos;
        transform.rotation = Quaternion.LookRotation(dir);
        player.SetDirection(dir);

        isCanShoot = true;
    }



    public void UpdateTurn(int targetIndex)
    {
        if (isFinish) return;

        if(targetIndex == index - 1)
        {
            index--;
        }

        //if(targetIndex < index && isCheckFail)
        //{
        //    isCheckFail = false;
        //    Fail();
        //    return;
        //}

        UpdateGround();
        SetDirectionPlayer();

        if (index == 0) CameraControl(false);

        index++;
        levelBarImage.fillAmount = (float)(index -1) / ((float)maxIndex - 2);

        isGroundFinal = (index >= listGrounds.Count - 1) ? true : false;

        if (isGroundFinal)
        {
            dirFocusRotateCamera = listGrounds[listGrounds.Count-1].transform.GetChild(1).transform.position - player.ball.transform.position;
            dirFocusRotateCamera.y = 0.0f;

            posX_Camera = player.ball.transform.position.x;
            player.trajectorySimulation.StartRotateDirection();
        }

        if (isFever && isGroundFinal == false)
        {
            if(isChangeBg)
            {
                player.ball.ChangeColorTrailFever();
                ColorManager.instance.ChangeBg();
                isChangeBg = false;
            }

            player.trajectorySimulation.JumpSimulation();

            feverNumber--;

            if(feverNumber <= 0)
            {
                isFever = false;
            }
            player.trajectorySimulation.arrow.SetActive(false);

        }
        else
        {
            player.ball.ChangeColorTrailNormal();

            isChangeBg = true;
            feverNumber = feverMaxNumber;
           // player.trajectorySimulation.arrow.SetActive(true);
            GameManager.Instance.player.ball.isFly = false;
            GameManager.Instance.player.trajectorySimulation.ActiveGolfClub();

            touchObject.SetActive(true);
        }    
    }

    private void LevelUp()
    {
        levelGame++;
        PlayerPrefs.SetInt("levelGame", levelGame);

        lvl = (levelGame >= levels.Length) ? Random.Range(levels.Length/2, levels.Length) : levelGame;
        StartCoroutine(C_Analytics_End());
    }


    public void OnTouchDown()
    {
        if (isCanShoot == false || isFinish) return;

        if (isGroundFinal)
        {
            player.trajectorySimulation.StopRotateDirection();
        }

        player.trajectorySimulation.StartForce();
        isShoot = true;
    }

    public void OnTouchUp()
    {
        if (isCanShoot == false || isFinish) return;

        isShoot = isCanShoot = false;
        player.trajectorySimulation.StopForce();
        touchObject.SetActive(false);
        isCheckFail = true;
    }

    private void RefreshTransformCameraMain()
    {
        Vector3 positionCameraMain = Camera.main.transform.localPosition;
        Vector3 rotationCameraMain = Camera.main.transform.localEulerAngles;

        positionCameraMain.y = currentPosY_Camera;
        positionCameraMain.z = currentPosZ_Camera;

        rotationCameraMain.x = currentRotX_Camera;

        Camera.main.transform.localPosition = positionCameraMain;
        Camera.main.transform.localEulerAngles = rotationCameraMain;


        Vector3 p1 = listGrounds[0].transform.position;
        Vector3 p2 = listGrounds[1].transform.position;
        p1.y = p2.y = 0;

        float posX = (p1.x + p2.x) / 2;

        if (p2.x > p1.x)
        {
            posX += -1;
        }
        else
        {
            posX -= -1;
        }

        float posY = 0;
        float posZ = listGrounds[0].transform.position.z;

        pivotCamera.transform.position = new Vector3(posX, posY, posZ);
        pivotCamera.transform.eulerAngles = Vector3.zero;
    }

    private void CameraControl(bool isLerp)
    {
        if (currentObject == null || targetObject == null) return;

        Vector3 p1 = currentObject.transform.position;
        Vector3 p2 = targetObject.transform.position;
        p1.y = p2.y = 0;

        float posX = (p1.x + p2.x) / 2;

        if(p2.x > p1.x)
        {
            posX += -1;
        }
        else
        {
            posX -= -1;
        }

        float posY = 0;
        float posZ = currentObject.transform.position.z;
        // y / 1.5 , z / 1.25   , rotate  x / 2

        Vector3 positionCameraMain = Camera.main.transform.localPosition;
        Vector3 rotationCameraMain = Camera.main.transform.localEulerAngles;

        if (isGroundFinal)
        {
            posX = posX_Camera;

            positionCameraMain.y = currentPosY_Camera / 1.5f;
            positionCameraMain.z = currentPosZ_Camera / 1.15f;

            rotationCameraMain.x = currentRotX_Camera /  1.5f;

            if (isFocusRotateCamera)
            {
                pivotCamera.transform.rotation = Quaternion.Lerp(pivotCamera.transform.rotation, Quaternion.LookRotation(dirFocusRotateCamera, Vector3.up), Time.deltaTime * speedLerpCamera * 0.75f);
            }
        }
        else
        {
            positionCameraMain.y = currentPosY_Camera;
            positionCameraMain.z = currentPosZ_Camera;

            rotationCameraMain.x = currentRotX_Camera;
        }

        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, positionCameraMain, Time.deltaTime * speedLerpCamera * 0.75f);
        Camera.main.transform.localEulerAngles = Vector3.Lerp(Camera.main.transform.localEulerAngles, rotationCameraMain, Time.deltaTime * speedLerpCamera);

   

        if(isLerp)
        {
            pivotCamera.transform.position = Vector3.Lerp(pivotCamera.transform.position, new Vector3(posX, posY, posZ), speedLerpCamera * Time.deltaTime);
        }
        else
        {
            pivotCamera.transform.position = new Vector3(posX, posY, posZ);
        }

        //Vector3 p3 = Vector3.zero;

        //if(p2.z >= p1.z)
        //{
        //    p3 = ((p1 + p2) / 2) + (Vector3.forward * (p2 - p1).magnitude) / 2;
        //}
        //else
        //{
        //    p3 = ((p1 + p2) / 2) + (Vector3.forward * -1 * (p2 - p1).magnitude) / 2;
        //}

        //Vector3 direction = p2 - p1;
        //Quaternion euler = Quaternion.LookRotation(direction);
        //       pivotCamera.transform.rotation = Quaternion.Lerp(pivotCamera.transform.rotation, euler, speedLerpCamera * Time.deltaTime);
    }

    public void Fail()
    {
        if (isFinish) return;

        StopAllCoroutines();
        isFinish = true;
        StartCoroutine(C_Fail());
    }

    private IEnumerator C_Fail()
    {
        Invoke("EndEvent_Analytic", 2);

        player.SetActiveTrail(false);
        touchObject.SetActive(false);
        Debug.Log("____FAIL____");

        if (isGroundFinal)
        {
            GameManager.Instance.HoleTextAnim("OUT OF GREEN");

            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return new WaitForSeconds(0.04f);
        }

       // UnityAdsManager.Instance.UpdateDeadToShowVideoAds();
        UIManager.instance.ShowMenu_Fail();
    }

    public void Complete()
    {
        if (isFinish) return;

        StopAllCoroutines();
        isFinish = true;
        StartCoroutine(C_Complete());
    }

    private IEnumerator C_Complete()
    {
        Invoke("EndLevelEvent_Analytic", 2);

        Physics.gravity = Vector3.up *  -80f;
        effectRippleFinish.SetActive(true);
        player.SetActiveTrail(false);
        touchObject.SetActive(false);
        Debug.Log("____COMPLETE____");
        yield return new WaitForSeconds(1.75f);

        int bodyPathsAmount = player.ball.snakeSimulation.listPaths.Count;
        PlayerPrefs.SetInt("bodyPaths", bodyPathsAmount);
        GameManager.Instance.bodyPathsAmount = bodyPathsAmount;
        isChangeColor = true;
        LevelUp();

        scoreCompletedText.text = gameScore.ToString();

        //UnityAdsManager.Instance.UpdateCompleteToShowVideoAds();
        UIManager.instance.ShowMenu_Complete();
    }

    public void CheckFail()
    {
        if (isFinish) return;

        StartCoroutine(C_CheckFail());
    }

    private IEnumerator C_CheckFail()
    {
        yield return new WaitForSeconds(.5f);

        while (player.ball.rigidbody.velocity.magnitude > 1f)
        {
            yield return null;
        }

        player.ball.rigidbody.velocity = Vector3.zero;
        Vector3 playerPos = player.ball.gameObject.transform.position;
        playerPos.y = 1f;

        if (player.ball.raycastIndex == 1)
        {
            player.ball.ScorePlusComplete(playerPos, 50, 4);
            Complete();
            GameManager.Instance.HoleTextAnim("ON GREEN");
        }
        else if (player.ball.raycastIndex == 2)
        {
            player.ball.ScorePlusComplete(playerPos, 50, 3);
            Complete();
            GameManager.Instance.HoleTextAnim("ON GREEN");
        }
        else if (player.ball.raycastIndex == 3)
        {
            player.ball.ScorePlusComplete(playerPos, 50, 2);
            Complete();
            GameManager.Instance.HoleTextAnim("ON GREEN");
        }
        else if( player.ball.raycastIndex == 4)
        {
            player.ball.ScorePlusComplete(playerPos, 50, 1);
            Complete();
            GameManager.Instance.HoleTextAnim("ON GREEN");
        }
        else
        {

            Fail();
        }
    }
    
    public void Effect_ElipseDual(Vector3 position)
    {
        GameObject effect = PoolManager.instance.GetObject(PoolManager.NameObject.ElipseDual);

        for(int i = 0; i < effect.transform.childCount; i++)
        {
            Color c = ColorManager.instance.setColor.colorElipse[Random.Range(0, ColorManager.instance.setColor.colorElipse.Length)];
            effect.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Renderer>().material.SetColor("_Color", c);
        }

        effect.transform.position = new Vector3(position.x, effect.transform.position.y, position.z);
        effect.SetActive(true);
    }

    public void ScorePlus(Vector3 position,int score,string congra,Color color)
    {
        GameObject scoreObject = PoolManager.instance.GetObject(PoolManager.NameObject.ScorePlus);
        scoreObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "+" + score;
        scoreObject.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = congra;
        scoreObject.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().color = color;

        scoreObject.transform.position = position;
        scoreObject.SetActive(true);

        PlusGameScore(score);
    }



    public void ScoreFinish(int score)
    {
        scoreAnim.SetTrigger("Awake");
        gameScore += score;
        PlayerPrefs.SetInt("scoreGame", gameScore);
        gameScoreText.text = gameScore.ToString();
    }

    public void PlusGameScore(int s)
    {
        scoreAnim.SetTrigger("Awake");

        gameScore += s;
        PlayerPrefs.SetInt("scoreGame", gameScore);

        gameScoreText.text = gameScore.ToString();
    }

    public void EffectPivotFinish(int num)
    {
        for(int i = 0; i < pivotFinishs.Count; i++)
        {
            if(i == num)
            {
                pivotFinishs[i].material.color = contact_finish_color;
            }
            else
            {
                pivotFinishs[i].material.color = pivot_finish_color[i];
            }
        }
    }

    public void SpawnGem(Vector3 posision)
    {
        GameObject gem = PoolManager.instance.GetObject(PoolManager.NameObject.Gem);
        gem.transform.position = posision;
        gem.SetActive(true);
    }

    public void EffectGem(GameObject target)
    {
        target.SetActive(false);

        GameObject effect = PoolManager.instance.GetObject(PoolManager.NameObject.GemEffect);
        Vector3 pos = target.transform.position;
        pos.y = 0.1f;
        effect.transform.position = pos;

        effect.SetActive(true);
    }

    public void EffectBodyPath(GameObject target)
    {
        GameObject effectBodyPath = PoolManager.instance.GetObject(PoolManager.NameObject.EffectBodyPath);
        effectBodyPath.transform.position = target.transform.position;
        effectBodyPath.SetActive(true);        
    }

    public void HoleTextAnim(string textString)
    {
        holeInOne.GetComponent<Text>().text = textString;
        holeInOne.SetActive(true);
    }
}