using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using HeroNetworkPlayer = NetworkPlayer;
using HeroNetworkManager = NetworkManager;


public class UIPFCController : MonoBehaviour {

    #region Singleton


    /// <summary>
    /// Gets the UIPFCController instance if it exists
    /// </summary>
    public static UIPFCController Instance
    {
        get;
        protected set;
    }

    public static bool InstanceExists
    {
        get { return Instance != null; }
    }

    #endregion



    public List<Button> buttonsList = new List<Button>();
    public List<Text> playerName = new List<Text>();
    public List<Text> playerScore = new List<Text>();
    public List<GameObject> animImageP1 = new List<GameObject>();
    public List<GameObject> animImageP2 = new List<GameObject>();
    public GameObject buttons;
    public GameObject winAnim;
    public GameObject looseAnim;
    public Text newRoundText;


    private int roundNumber = 1;
    private Animator anim;

    private HeroNetworkPlayer localPlayer;
    public HeroNetworkPlayer m_localPlayer
    { get { return localPlayer; } }

    private HeroNetworkPlayer otherPlayer;
    public HeroNetworkPlayer m_otherPlayer
    { get { return otherPlayer; } }

    /// <summary>
    /// Initialize our singleton
    /// </summary>
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

    /// <summary>
    /// Clear the singleton
    /// </summary>
    protected void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    public void AddPlayer(HeroNetworkPlayer player)
    {
        Debug.Log("AddPlayer");


        if (player.hasAuthority)
        {
            localPlayer = player;
        }
        else
            otherPlayer = player;

    }



    private void Start()
    {
        anim = GetComponent<Animator>();
    }



    public void OnClickButton(int choice)// 1 = rock, 2 = paper, 3 = scissors
    {
        localPlayer.GetPFCChoice(choice);
    }


    



    public void UpdateButtons()
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            if (buttonsList[i].interactable)
                buttonsList[i].interactable = false;
            else
                buttonsList[i].interactable = true;
        }
    }


    public void PFCWinAnim(int p1Choice, int p2Choice, int winner)
    {

        Debug.Log("PFCWinAnim");

        buttons.SetActive(false);

        Debug.Log("p1Choice -1 = " + (p1Choice - 1));
        Debug.Log("p2Choice -1 = " + (p2Choice - 1));
        Debug.Log("animImageP1[p1Choice -1] = " + (animImageP1[p1Choice - 1]));
        Debug.Log("animImageP2[p2Choice -1] = " + (animImageP2[p2Choice - 1]));

        animImageP1[p1Choice -1].SetActive(true);
        animImageP2[p2Choice -1].SetActive(true);

        anim.Play("PFCAnim");

        if (winner == -1)
        {
            anim.SetTrigger("draw");
        }
        else if (winner == localPlayer.playerID)
        {
            anim.SetTrigger("win");
        }
        else 
        {
            anim.SetTrigger("loose");
        }

        Invoke("EndRound", 4f);

    }

    private void EndRound()
    {
        Debug.Log("EndRound");

        localPlayer.PFCChoice = 0;
        otherPlayer.PFCChoice = 0;

        
        if (localPlayer.hasAuthority && localPlayer.GetPFSScore >= 2)
        {
            Debug.Log("localPlayer.hasAuthority && localPlayer.GetPFSScore >= 2)");
            localPlayer.CmdGameState();
            return;
        }


        roundNumber++;
        newRoundText.text = "ROUND " + roundNumber + " START";
        anim.SetTrigger("newRound");

        Invoke("StartNewRound", 2f);
    }

    private void StartNewRound()
    {
        anim.Play("Default");

        for (int i = 0; i < animImageP1.Count; i++)
        { 
            if (animImageP1[i].activeSelf)
                animImageP1[i].SetActive(false);
        }
        for (int i = 0; i < animImageP2.Count; i++)
        {
            if (animImageP2[i].activeSelf)
                animImageP2[i].SetActive(false);
        }

        buttons.SetActive(true);

        UpdateButtons();
    }

}
