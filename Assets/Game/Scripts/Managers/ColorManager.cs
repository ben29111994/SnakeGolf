using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;


    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public List<SetColor> listSetColors = new List<SetColor>();
    public int number;
    [Header("References")]
    public Material glassMaterial;
    public Material tubeMaterial;
    public Material dotMaterial;
    public SpriteRenderer sprDot;
    public SetColor setColor;

    [System.Serializable]
    public class SetColor
    {
        public Color colorGlass;
        public Color colorTube;
        public Color colorDot;
        public Color[] colorElipse;
        public int ignoreSea;
    }

    private void Start()
    {
        number = 0;

        ChangeColor();
    }

    public void ChangeColor()
    {
        setColor = listSetColors[number];
        glassMaterial.color = setColor.colorGlass;
        tubeMaterial.color = setColor.colorTube;
        dotMaterial.color = setColor.colorDot;
        sprDot.color = setColor.colorDot;

        ChangeBg();

        number++;
        if (number >= listSetColors.Count) number = 0;
    }

    [Header("BackGround Plane")]
    public GameObject[] listBgs;
    private GameObject currentBg;
    private GameObject targetBg;
    private int indexBg;

    public void ChangeBg()
    {
        StartCoroutine(C_ChangeBg());
    }

    private IEnumerator C_ChangeBg()
    {
        currentBg = listBgs[indexBg];

        for(int i = 0; i < 1; i++)
        {
            int r = Random.Range(1, listBgs.Length);

            if (r == indexBg || r == setColor.ignoreSea)
            {
                i--;
            }
            else
            {
                indexBg = r;
            }
        }

        targetBg = listBgs[indexBg];

        Vector3 pos = targetBg.transform.position;
        pos.y = -2.98f;
        targetBg.transform.position = pos;

        targetBg.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        currentBg.SetActive(false);

        Vector3 poss = targetBg.transform.position;
        poss.y = -3f;
        targetBg.transform.position = poss;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeBg();
        }
    }

}
