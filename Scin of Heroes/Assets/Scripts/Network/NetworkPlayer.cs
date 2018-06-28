using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using HeroNetworkManager = NetworkManager;
using HeroNetworkPlayer = NetworkPlayer;


public class NetworkPlayer : NetworkBehaviour
{

    public event Action<HeroNetworkPlayer> syncVarsChanged;

    //Server only event
    public event Action<HeroNetworkPlayer> activesPlayersEvent;

    //Server only event
    public event Action<HeroNetworkPlayer> choiceDone;




    [SyncVar(hook = "InitUI")]
    private int PFCScore = 0;
    public int GetPFSScore
    { get { return PFCScore; } }



    public int PFCChoice = 0;
    public GameObject go;

    [SerializeField]
    protected GameObject m_PFCPlayerUI;
    [SerializeField]
    protected GameObject hero;

    public SceneFader sceneFader;

    private int winner;

    private Dictionary<string, NewNetworkedPool> poolDictionary;


    [SyncVar]
    private int m_PlayerID;

    private HeroNetworkManager m_NetManager;


    /// <summary>
    /// Gets this player's id
    /// </summary>
    public int playerID
    { get { return m_PlayerID; } }


    ///// <summary>
    ///// Get the PFCPlayerObject associate with this player
    ///// </summary>
    //public PFCPlayer PFCPlayer
    //{ get; private set; }

    public NetworkPlayer s_LocalPlayer
    { get; private set; }




    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            if (hasAuthority)
            {
                CmdGameState();

            }
        }
    }



    [Server]
    public void SetPlayerID(int playerID)
    {
        this.m_PlayerID = playerID;
    }

    /// <summary>
    /// Register us with the NetworkManager
    /// </summary>
    [Client]
    public override void OnStartClient()
    {
        DontDestroyOnLoad(this.gameObject);

        if (m_NetManager == null)
            m_NetManager = HeroNetworkManager.Instance;

        base.OnStartClient();

        Debug.Log("Client Network Player start");

        m_NetManager.RegisterNetworkPlayer(this);
    }


    /// <summary>
    /// Set initial values
    /// </summary>
    [Client]
    public override void OnStartLocalPlayer()       // called automatically on start and only on the local player
    {
        base.OnStartLocalPlayer();
        Debug.Log("OnStartLocalPlayer");
        s_LocalPlayer = this;


        CmdActivesPlayers();


        //recupère le nom, la liste des unités sélectionnée... dans un fichier de sauvegarde (playerDataManager)
    }

    [Command]
    private void CmdActivesPlayers()
    {
        Debug.Log("CmdActivesPlayers");

        //connected = true;

        if (m_NetManager.playerCount >= 2)
        {
            m_NetManager.ProgressToPFCScene();
        }

    }

    [Command]
    public void CmdGameState()
    {
        m_NetManager.ProgressToGame();
    }

    [ClientRpc]
    public void RpcFadeIn()
    {
        //sceneFader.FadeTo( );
    }


    /// <summary>
    /// Called when we enter MapRomainScene
    /// </summary>
    public void OnEnterMapRomainScene()
    {
        poolDictionary = PoolManager.Instance.poolDictionnary;

        if (!hasAuthority)
            return;

        Debug.Log("OnEnterMapRomainScene", this);

        CmdInstantiateHero();
    }

    [Command]
    private void CmdInstantiateHero()
    {
        GameObject _hero = Instantiate(hero, new Vector3(0, 0, -63), Quaternion.identity);
        NetworkServer.Spawn(_hero);
    }







    public void RegisterAgentSelectedList()
    {
        if (!hasAuthority)
            return;

        Debug.Log("RegisterAgentSelectedList", this);

        foreach (string agent in AgentSelector.Instance.selectedAgentsString)
        {
            CmdRegisterAgentSelectedList(agent);
        }
    }

    [Command]
    private void CmdRegisterAgentSelectedList(string agent)
    {
        Debug.Log("CmdRegisterAgentSelectedList", this);

        RpcRegisterAgentSelectedList(agent);
    }

    [ClientRpc]
    private void RpcRegisterAgentSelectedList(string agent)
    {
        Debug.Log("RpcRegisterAgentSelectedList", this);
        Debug.Log("agent = " + agent);



        if (poolDictionary[agent] != null)
        {
            Debug.Log("poolDictionary[agent] != null");

            poolDictionary[agent].poolSize = 23;
            poolDictionary[agent].Init();
        }
    }



    /// <summary>
    /// Called when we enter PFCScene
    /// </summary>
    public void OnEnterPFCScene()
    {
        Debug.Log("OnEnterPFCScene");

        UIPFCController.Instance.AddPlayer(this);

        InitUI(0);
    }



    public void InitUI(int newScore)
    {
        Debug.Log("PFCScore's hook");

        PFCScore = newScore;


        for (int i = 0; i < m_NetManager.connectedPlayers.Count; i++)
        {
            UIPFCController.Instance.playerName[i].text = "Player " + m_NetManager.connectedPlayers[i].m_PlayerID.ToString();
            UIPFCController.Instance.playerScore[i].text = m_NetManager.connectedPlayers[i].PFCScore.ToString();
        }
    }


    public override void OnNetworkDestroy()
    {
        Debug.Log("NetworkPlayer call OnNetworkDestroy");

        base.OnNetworkDestroy();

        if (m_NetManager != null)
        {
            m_NetManager.DeregisterNetworkPlayer(this);
        }
    }




    public void GetPFCChoice(int choice)
    {
        CmdSetPFCChoice(choice);

        UIPFCController.Instance.UpdateButtons();

        CmdAreAllChoicesDone();
    }



    [Command]
    private void CmdSetPFCChoice(int choice)
    {
        Debug.Log("CmdSetPFCChoice");
        PFCChoice = choice;
    }



    [Command]
    private void CmdAreAllChoicesDone()
    {
        foreach (HeroNetworkPlayer player in HeroNetworkManager.Instance.connectedPlayers)
        {
            if (player.PFCChoice == 0)
            {
                return;
            }
        }
        CmdCompareChoices();
    }





    [Command]
    private void CmdCompareChoices()
    {
        Debug.Log("CompareChoices");
        int localChoice = UIPFCController.Instance.m_localPlayer.PFCChoice;
        int otherChoice = UIPFCController.Instance.m_otherPlayer.PFCChoice;

        winner = -1;


        if (localChoice == otherChoice)
        {
            Debug.Log("DRAW !!!!");
        }
        else
        {
            switch (localChoice)
            {
                case 1:
                    if (otherChoice == 2)
                    {
                        UIPFCController.Instance.m_otherPlayer.PFCScore++;
                        winner = 1;
                    }
                    if (otherChoice == 3)
                    {
                        UIPFCController.Instance.m_localPlayer.PFCScore++;
                        winner = 0;
                    }
                    break;

                case 2:
                    if (otherChoice == 1)
                    {
                        UIPFCController.Instance.m_localPlayer.PFCScore++;
                        winner = 0;
                    }
                    if (otherChoice == 3)
                    {
                        UIPFCController.Instance.m_otherPlayer.PFCScore++;
                        winner = 1;
                    }
                    break;

                case 3:
                    if (otherChoice == 1)
                    {
                        UIPFCController.Instance.m_otherPlayer.PFCScore++;
                        winner = 1;
                    }
                    if (otherChoice == 2)
                    {
                        UIPFCController.Instance.m_localPlayer.PFCScore++;
                        winner = 0;
                    }
                    break;
            }
        }

        RpcLaunchPFCAnim(localChoice, otherChoice, winner);

        localChoice = 0;
        otherChoice = 0;
    }

    [ClientRpc]
    private void RpcLaunchPFCAnim(int p1Choice, int p2Choice, int winner)
    {
        Debug.Log("RpcLaunchPFCAnim");
        UIPFCController.Instance.PFCWinAnim(p1Choice, p2Choice, winner);
    }



    [Command]
    public void CmdSpawnFromPool(int indexUniteToSpawn, Vector3 position)
    {
        NewNetworkedPool netPool = PoolManager.Instance.poolDictionnary[AgentSelector.Instance.selectedAgentsString[indexUniteToSpawn]];
        go = netPool.GetFromPool(position);
        NetworkServer.Spawn(go);
    }

    [Command]
    public void CmdReturnToPool()
    {

    }

}
