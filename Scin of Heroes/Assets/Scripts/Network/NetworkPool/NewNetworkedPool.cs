
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class NewNetworkedPool : NetworkBehaviour
{
    public int poolSize = 0;
    public GameObject m_Prefab;
    public List<GameObject> m_Pool = new List<GameObject>();
    public string poolName;

    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);

    public void Init()
    {
        assetId = m_Prefab.GetComponent<NetworkIdentity>().assetId;

        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);

        if (isServer)
            CmdInit();
    }


    [Command]
    public void CmdInit()
    {
       
       
        for (int i = 0; i < poolSize; ++i)
        {
            GameObject obj = (GameObject)Instantiate(m_Prefab);
            obj.transform.parent = this.transform;
            m_Pool.Add(obj);
            NetworkServer.Spawn(m_Pool[i]);


            m_Pool[i].SetActive(false);
        }

      
    }


    //[ClientRpc]
    //private void TurnOffObject()
    //{

    //}

    public GameObject GetFromPool(Vector3 position)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                Debug.Log("Activating GameObject " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
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
        Debug.Log("Re-pooling GameObject " + spawned.name);
        spawned.SetActive(false);
    }
}