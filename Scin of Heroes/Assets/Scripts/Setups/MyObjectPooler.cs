using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool willGrow = true;
    }

    #region Singleton
    public static MyObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> poolDictionary; // associate a game object's list to the prefab name
    public Dictionary<string, Pool> poolToListDictionary;      // associate a pool to the prefab name

    void Start ()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        poolToListDictionary = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++) // Instantiate as unactives prefabs as count value and place them in a list
            {
                GameObject obj = (GameObject)Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = this.transform;
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectPool); 
            poolToListDictionary.Add(pool.tag, pool); 


        }
	}


 

    public GameObject SpawnFromPool(GameObject objToSpawn)
    {
        string name = objToSpawn.name.ToString();

        if (!poolDictionary.ContainsKey(name))
        {
            Debug.LogWarning("Pool with tag " + name + " doesn't exist!");
            return null;
        }



        List<GameObject> thisPoolList = poolDictionary[name];


        for (int i = 0; i < thisPoolList.Count; i++)
        {
            if (!thisPoolList[i].activeInHierarchy)
            {
                return thisPoolList[i];
            }
        }

        if (poolToListDictionary[name].willGrow)
        {
            GameObject obj = (GameObject)Instantiate(poolToListDictionary[name].prefab);
            obj.transform.parent = this.transform;
            obj.SetActive(false);
            thisPoolList.Add(obj);
            return obj;
        }

        Debug.Log("Pool is empty and willGrow is false");
        return null;
    }


    public GameObject SpawnFromPoolAt(GameObject objToSpawn, Vector3 position, Quaternion rotation)
    {
        GameObject _objToSpawn = SpawnFromPool(objToSpawn);
        _objToSpawn.transform.position = position;
        _objToSpawn.transform.rotation = rotation;
        _objToSpawn.SetActive(true);

        return _objToSpawn;
    }


    public void ReturnToPool (GameObject obj)
    {
        obj.SetActive(false);
    }


}
