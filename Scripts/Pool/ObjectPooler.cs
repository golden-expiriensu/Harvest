using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public PoolObjectId id;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }

    public List<Pool> pools;
    Dictionary<PoolObjectId, Queue<GameObject>> poolDictionary;

    public static ObjectPooler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public enum PoolObjectId
    {
        FlyingNumberHarvest,
        FlyingNumberDamage,
        FlyingNumberHeal,
        FlyingNumberFuel
    }

    void Start()
    {
        poolDictionary = new Dictionary<PoolObjectId, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.id, objectPool);
        }
    }

    public GameObject GetObjectFromPool(PoolObjectId id)
    {
        GameObject obj = poolDictionary[id].Dequeue();

        obj.SetActive(true);
        poolDictionary[id].Enqueue(obj);

        return obj;
    }
}
