using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByTime : MonoBehaviour
{
    public float timeBeforeDisable;
    private float _timeBeforeDisable;

  

    private void OnEnable()
    {
        _timeBeforeDisable = timeBeforeDisable;
    }

    private void Update()
    {
        _timeBeforeDisable -= Time.deltaTime;
        if (_timeBeforeDisable <= 0)
        {
            PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
        }
    }

   


}
