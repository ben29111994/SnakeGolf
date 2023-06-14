using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSimulation : MonoBehaviour
{
    public GameObject head;
    public float mDistance;
    [HideInInspector]
    public List<Path> listPaths = new List<Path>();

    private float scaleBody;

    [Header("ColorSnake")]
    private int currentIndexSetColor;
    public int indexColor;
    public int indexSetColor;
    public SetColor currentSetColor;
    public List<SetColor> setColores = new List<SetColor>();
    public Material[] bodyMaterials;

    [System.Serializable]
    public class SetColor
    {
        public int bodyColorAmount;
        public Color[] colorBodies;
    }

    [System.Serializable]
    public class Path
    {
        public float minDistance;
        public Transform bodyPath;
        public Queue<Vector3> listPosition = new Queue<Vector3>();
    }

    public void ChangeSetColorBody()
    {
        indexColor = 0;
        indexSetColor = 0;

        int r = 0;

        for(int i = 0; i < 1; i++)
        {
            r = Random.Range(0, setColores.Count);
            if (r == currentIndexSetColor)
            {
                i--;
            }
        }

        currentIndexSetColor = r;
        currentSetColor = setColores[r];
    }

    private void SetColorBody(Renderer renderer)
    {
        bodyMaterials[indexSetColor].color = currentSetColor.colorBodies[indexSetColor];
        renderer.material = bodyMaterials[indexSetColor];

        indexColor++;
        if (indexColor >= currentSetColor.bodyColorAmount)
        {
            indexColor = 0;
            indexSetColor++;

            if (indexSetColor >= currentSetColor.colorBodies.Length)
            {
                indexSetColor = 0;
            }
        }
    }

    private void GenerateHead()
    {
        Path path = new Path();
        path.bodyPath = head.transform;

        Renderer renderer = path.bodyPath.GetComponent<Renderer>();
        SetColorBody(renderer);

        listPaths.Add(path);
    } 

    public void GenerateBodyPath(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            scaleBody -= 0.0075f;
            GameObject bodyPath = PoolManager.instance.GetObject(PoolManager.NameObject.BodyPath);
            bodyPath.transform.SetParent(transform);
            bodyPath.transform.position = listPaths[listPaths.Count - 1].bodyPath.position;
            bodyPath.transform.localScale = Vector3.one * scaleBody;
            bodyPath.SetActive(true);

            Path path = new Path();
            path.bodyPath = bodyPath.transform;
            path.minDistance = 0;

            Renderer renderer = path.bodyPath.GetComponent<Renderer>();
            SetColorBody(renderer);

            listPaths.Add(path);
        }
    }

    public void SetDistance()
    {
        for (int i = 1; i < listPaths.Count; i++)
        {
            Path path = listPaths[i];
            path.minDistance = mDistance;
        }
    }


    public Transform parentBodyPath;

    public void RefreshBodyPath()
    {
        scaleBody = 1;

        for (int i = 1; i < listPaths.Count; i++)
        {
            listPaths[i].bodyPath.gameObject.SetActive(false);
            listPaths[i].bodyPath.SetParent(parentBodyPath);
        }

        listPaths.Clear();
        GenerateHead();

        // int bodyPathsAmount = PlayerPrefs.GetInt("bodyPaths");
        int bodyPathsAmount = 19;
        GenerateBodyPath(bodyPathsAmount);
        GameManager.Instance.bodyPathsAmount = bodyPathsAmount;
    }


    public void UpdateStep(bool isAnim)
    {
        if (isAnim == false)
        {
            for (int i = 1; i < listPaths.Count; i++)
            {
                listPaths[i].bodyPath.position = listPaths[i - 1].bodyPath.position;
            }

            return;
        }

        if (listPaths.Count == 0) return;

        if (listPaths.Count >= 2)
        {
            Vector3[] posArray = listPaths[0].listPosition.ToArray();

            if (posArray.Length != 0)
            {
                if (listPaths[0].bodyPath.position != posArray[posArray.Length - 1])
                {
                    listPaths[0].listPosition.Enqueue(listPaths[0].bodyPath.position);
                }
            }
            else
            {
                listPaths[0].listPosition.Enqueue(listPaths[0].bodyPath.position);
            }
        }

        if (listPaths[0].listPosition.Count != 0)
        {
            for (int i = 1; i < listPaths.Count; i++)
            {
                Path curBodyPath = listPaths[i];
                Path preBodyPath = listPaths[i - 1];

                Vector3 dir = preBodyPath.bodyPath.position - curBodyPath.bodyPath.position;
                if (dir != Vector3.zero)
                    curBodyPath.bodyPath.rotation = Quaternion.LookRotation(dir);

                float distance = Vector3.Distance(curBodyPath.bodyPath.position, preBodyPath.bodyPath.position);

                float minDis = (GameManager.Instance.isGroundFinal) ? curBodyPath.minDistance : curBodyPath.minDistance;

                if (distance > minDis)
                {
                    if (curBodyPath.minDistance != -1)
                        curBodyPath.minDistance = -1;

                    if (preBodyPath.listPosition.Count != 0)
                        curBodyPath.bodyPath.position = preBodyPath.listPosition.Dequeue();

                    if (i < listPaths.Count - 1)
                    {
                        Vector3[] posArray = curBodyPath.listPosition.ToArray();

                        if (posArray.Length != 0)
                        {
                            if (curBodyPath.bodyPath.position != posArray[posArray.Length - 1])
                            {
                                curBodyPath.listPosition.Enqueue(curBodyPath.bodyPath.position);
                            }
                        }
                        else
                        {
                            curBodyPath.listPosition.Enqueue(curBodyPath.bodyPath.position);
                        }
                    }
                }
            }
        }
    }



}
