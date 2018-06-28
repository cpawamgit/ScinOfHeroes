using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hero : Unite
{
    //public int AAdamage; //auto attack damage

    private void Start()
    {
        InitPool();
    }

    private void InitPool()
    {
        GameManager2.Instance.playerInGameScene++;
        GameManager2.Instance.InitPool();
        Debug.Log("GameManager2.Instance.playerInGameScene = " + GameManager2.Instance.playerInGameScene);
    }

}
