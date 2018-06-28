
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class NewNetworkedPool : MonoBehaviour
{
    public int poolSize = 0;
    public GameObject m_Prefab;
    public List<GameObject> m_Pool = new List<GameObject>();
    public string poolName;
    public bool willGrow = true;


    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);

    public void Init()
    {
        assetId = m_Prefab.GetComponent<NetworkIdentity>().assetId;

        for (int i = 0; i < poolSize; ++i)
        {
            GameObject obj = (GameObject)Instantiate(m_Prefab);
            obj.transform.parent = this.transform;
            obj.name = m_Prefab.name;
            m_Pool.Add(obj);
            obj.SetActive(false);
        }

        if (m_Prefab.GetComponent<Unite>() != null)
        {
            if (m_Prefab.GetComponent<Unite>().unitData != null)
            {
                foreach (GameObject go in m_Prefab.GetComponent<Unite>().unitData.objectToPool)
                {
                    if (PoolManager.Instance.poolDictionnary[go.name] != null)
                    {
                        Debug.Log("poolDictionary[agent] != null");

                        PoolManager.Instance.poolDictionnary[go.name].poolSize = 23;
                        PoolManager.Instance.poolDictionnary[go.name].Init();
                    }
                }
            }
        }
        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
    }



   

    public GameObject GetFromPool(Vector3 position)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating GameObject " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
        }

        if (willGrow)
        {
            //Debug.Log("Adding to the Pool and Activating GameObject " + m_Prefab.name + " at " + position);
            GameObject obj = (GameObject)Instantiate(m_Prefab);
            obj.transform.parent = this.transform;
            m_Pool.Add(obj);
            obj.name = m_Prefab.name;
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }


        Debug.LogError("Could not grab GameObject from pool, nothing available");
        return null;
    }

    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        return GetFromPool(position);
    }

    public void UnSpawnObject(GameObject spawned)
    {
        //Debug.Log("Re-pooling GameObject " + spawned.name);
        spawned.SetActive(false);
    }

    
}