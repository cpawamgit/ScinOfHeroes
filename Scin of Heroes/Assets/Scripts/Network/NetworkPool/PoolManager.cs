using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{


    public Dictionary<string, NewNetworkedPool> poolDictionnary = new Dictionary<string, NewNetworkedPool>();


    #region Singleton
    public static PoolManager Instance;

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDisable()
    {
        Instance = null;
    }

    #endregion

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            NewNetworkedPool pool = transform.GetChild(i).GetComponent<NewNetworkedPool>();
            poolDictionnary.Add(pool.poolName, pool);
        }
    }


}
