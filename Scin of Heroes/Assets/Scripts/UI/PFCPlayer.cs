using UnityEngine;
using UnityEngine.UI;
using HeroNetworkManager = NetworkManager;
using HeroNetworkPlayer = NetworkPlayer;
using UnityEngine.Networking;

public class PFCPlayer : NetworkBehaviour
{
    [HideInInspector]
    public HeroNetworkPlayer m_NetPlayer;
    public Text nameText;
    public Text scoreText;

    [HideInInspector]
    public int score = 0;

    [HideInInspector]
    [SyncVar(hook ="test")]
    public int PFCChoice = 0;

    void test(int choice)
    {
        Debug.Log("test : choice = "+ choice, this);
    }


    

    public void Init(HeroNetworkPlayer netPlayer)
    {
        Debug.Log("Init PFCPlayer ma gueule");
        this.m_NetPlayer = netPlayer;

        nameText.text = "Player " + (netPlayer.playerID + 1).ToString();
        scoreText.text = score.ToString();


   

        //if (netPlayer != null)
        //{
        //    netPlayer.syncVarsChanged += OnNetworkPlayerSyncVarChanged;
        //}


        //UIPFCController.Instance.AddPlayer(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("PFCChoice = " + PFCChoice);
        }
    }



    private void UpdateScore(int newScore)
    {
        Debug.Log("UpdateScore  hook");

        Debug.Log("PFCPlayer.score dans UpdateScore hook= " + newScore);


        scoreText.text = newScore.ToString();

        //if (winner.hasAuthority)
        //{
        //    /// WOUAI G GAGNE
        //}
        //else { ooooooooooh}
    }


    //private void OnNetworkPlayerSyncVarChanged(HeroNetworkPlayer player)
    //{

    //}



}