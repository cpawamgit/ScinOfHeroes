using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    #region Singleton
    public static PlayerStats Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public static GameManager2 GameManager;
    public GameManager2 gameManager;



    private static int money;
    public int startMoney = 400;
  
    public static float Lives;
    public int startLives = 20;

    public UnitesMenu unitesMenu;
    public TowersMenu towersMenu;

    public static int Rounds;


    void Start()
    {
        GameManager = gameManager;
        money = startMoney;
        Lives = startLives;
        Rounds = 0;

        InvokeRepeating("MoneyOnTime", 0, 1);
    }


    public int Money
    {
        get
        {
            return money;
        }
    }


    public static void DecreaseLife()
    {
        Lives--;
        Lives = Mathf.Clamp(Lives, 0f, Mathf.Infinity);

        if (Lives <= 0)
        {
            GameManager.GameOver();
        }
    }

    public void ChangeMoney(int amount)
    {
        money += amount;
        unitesMenu.CheckMoney();
        towersMenu.CheckMoney();
    }

    private void MoneyOnTime()
    {
        ChangeMoney(1);
    }



}
