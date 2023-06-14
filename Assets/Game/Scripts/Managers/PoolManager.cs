using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
        GenerateObjectPool();
    }


    [Header("Pool Manager")]
    public List<ObjectPool> ObjectPools = new List<ObjectPool>();
    
    public enum NameObject
    {
        Dot,
        ElipseDual,
        ScorePlus,
        Gem,
        GemEffect,
        BodyPath,
        EffectBodyPath,
        ScorePlusBodyPath,
        NearMiss,
        Balloon,
        Shark
    }

    [System.Serializable]
    public class ObjectPool
    {
        public Transform parent;
        public int amount;
        public GameObject objectPrefab;
        public NameObject nameObject;

        [HideInInspector]
        public List<GameObject> listObject = new List<GameObject>();
    }

    public void GenerateObjectPool()
    {
        int count = ObjectPools.Count;

        for(int i = 0; i < count; i++)
        {
            int amount = ObjectPools[i].amount;
            GameObject prefab = ObjectPools[i].objectPrefab;
            Transform parent = ObjectPools[i].parent;

            for(int j = 0; j < amount; j++)
            {
                GameObject objectClone = Instantiate(prefab, parent);
                objectClone.SetActive(false);
                ObjectPools[i].listObject.Add(objectClone);
            }
        }
    }

    public GameObject GetObject(NameObject name)
    {
        int count = ObjectPools.Count;
        ObjectPool objectPool = null;

        for (int i = 0; i < count; i++)
        {
            if (ObjectPools[i].nameObject == name)
            {
                objectPool = ObjectPools[i];
            }
        }

        if (objectPool == null) return null;

        int childCount = objectPool.listObject.Count;

        for (int i = 0; i < childCount; i++)
        {
            GameObject childObject = objectPool.listObject[i].gameObject;
            if (childObject.activeInHierarchy == false)
            {
                return childObject;
            }
        }

        GameObject objectClone = Instantiate(objectPool.objectPrefab, objectPool.parent);
        objectClone.SetActive(false);
        objectPool.listObject.Add(objectClone);
        return objectClone;
    }

    public void RefreshDot()
    {
        for (int i = 0; i < ObjectPools.Count; i++)
        {
            if (ObjectPools[i].nameObject == NameObject.Dot)
            {
                for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
                {
                    ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
                }
            }
            //else if (ObjectPools[i].nameObject == NameObject.ItemBomb1)
            //{
            //    for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
            //    {
            //        ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
            //    }
            //}
            //else if (ObjectPools[i].nameObject == NameObject.ExplosionBomb0)
            //{
            //    for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
            //    {
            //        ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
            //    }
            //}
            //else if (ObjectPools[i].nameObject == NameObject.BulletBomb1)
            //{
            //    for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
            //    {
            //        ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
            //    }
            //}

        }
    }

   

    public void RefreshBodyPath()
    {
        for (int i = 0; i < GameManager.Instance.player.ball.snakeSimulation.transform.childCount; i++)
        {
            Debug.Log(i);
            GameManager.Instance.player.ball.snakeSimulation.transform.GetChild(i).gameObject.SetActive(false);
            GameManager.Instance.player.ball.snakeSimulation.transform.GetChild(i).gameObject.transform.SetParent(ObjectPools[5].parent);
        }
    }

    public void RefreshAnimals()
    {
        for (int i = 0; i < ObjectPools.Count; i++)
        {
            if (ObjectPools[i].nameObject == NameObject.Balloon)
            {
                for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
                {
                    ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
                }
            }
        }
    }

    public void RefreshGem()
    {
        for (int i = 0; i < ObjectPools.Count; i++)
        {
            if (ObjectPools[i].nameObject == NameObject.Gem)
            {
                for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
                {
                    ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
                }
            }
        }
    }
}
