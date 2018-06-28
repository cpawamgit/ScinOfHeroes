using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class Hero : Unite
{
    //public int AAdamage; //auto attack damage

    public List<Skills> skills;

    private void Start()
    {
        InitPool();



        // if client authority
        //a modifier 
        HeroUI.Instance.hero = this;
        HeroUI.Instance.Init();
    }

    private void InitPool()
    {
        GameManager2.Instance.playerInGameScene++;
        GameManager2.Instance.InitPool();
        Debug.Log("GameManager2.Instance.playerInGameScene = " + GameManager2.Instance.playerInGameScene);
    }

    protected override void Die()
    {
        isDead = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }

        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<HeroMovement>().enabled = false;
        gameObject.transform.position = new Vector3(-500, -500, -500);


    }
}
