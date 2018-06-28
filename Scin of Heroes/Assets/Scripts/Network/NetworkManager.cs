using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking.Match;
using System;



public enum ActualSceneState
{
    None,
    Game,
    Menu,
    PFC,
}

public class NetworkManager : UnityEngine.Networking.NetworkManager
{

    #region Singleton


    /// <summary>
    /// Gets the NetworkManager instance if it exists
    /// </summary>
    public static NetworkManager Instance
    {
        get;
        protected set;
    }

    public static bool InstanceExists
    {
        get { return Instance != null; }
    }

    #endregion


    /// <summary>
    /// Called on all clients when a player joins
    /// </summary>
    public event Action<NetworkPlayer> playerJoined;
    /// <summary>
    /// Called on all clients when a player leaves
    /// </summary>
    public event Action<NetworkPlayer> playerLeft;


    private ActualSceneState m_ActualSceneState;

    public string sceneToLoad;
    [SerializeField]
    public GameObject m_NetworkPlayerPrefab;


    /// <summary>
    /// Collection of all connected players
    /// </summary>
    public List<NetworkPlayer> connectedPlayers
    { get; private set; }

    /// <summary>
    /// Maximum number of players in a multiplayer game
    /// </summary>
    [SerializeField]
    protected int m_MultiplayerMaxPlayers = 2;


    /// <summary>
    /// Called on clients and server when the scene changes
    /// </summary>
    public event Action<bool, string> sceneChanged;


    /// <summary>
    /// Gets whether or not we're a server
    /// </summary>
    public static bool s_IsServer
    { get { return NetworkServer.active; } }


    /// <summary>
    /// Gets current number of connected player
    /// </summary>
    public int playerCount
    {
        get
        {
            return s_IsServer ? numPlayers : connectedPlayers.Count;
        }
    }


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

            connectedPlayers = new List<NetworkPlayer>();
        }
    }




    //GameMaster.Instance.SetPlayersConnectedList(connectedPlayers);



    private void Update()
    {
        if (m_ActualSceneState != ActualSceneState.None)
        {
            if (m_ActualSceneState == ActualSceneState.PFC) //read only by the server
            {
                ServerChangeScene(sceneToLoad);
                matchMaker.SetMatchAttributes(matchInfo.networkId, false, 0, (success, info) => Debug.Log("Match hidden")); //Unlist
            }
            else if (m_ActualSceneState == ActualSceneState.Game)
            {
                m_ActualSceneState = ActualSceneState.None;
                Debug.Log("ServerChangeScene called dans update");
                ServerChangeScene("MapRomainScene");
            }
        }

        m_ActualSceneState = ActualSceneState.None;
    }



    public void ProgressToPFCScene()
    {
        m_ActualSceneState = ActualSceneState.PFC;
    }


    public void ProgressToGame()
    {
        Debug.Log("ProgressToGame()");
        m_ActualSceneState = ActualSceneState.Game;
    }

    /// <summary>
    /// Register network players so we have all of them
    /// </summary>
    public void RegisterNetworkPlayer(NetworkPlayer newPlayer)
    {
        Debug.Log("Player joined");
        connectedPlayers.Add(newPlayer);

        newPlayer.choiceDone += OnPlayerChoiceDone;

        string activeScene = SceneManager.GetActiveScene().name;

        if (s_IsServer)
        {
            UpdatePlayersIDs();
        }

        if (activeScene == "PFCscene")
            newPlayer.OnEnterPFCScene();
        else if (activeScene == "MapRomainScene")
        {
            Debug.Log(" else if (activeScene == MapRomainScene)");
            newPlayer.OnEnterMapRomainScene();
        }

        if (playerJoined != null)
            playerJoined(newPlayer);
    }

    /// <summary>
    /// Deregister network players
    /// </summary>
    public void DeregisterNetworkPlayer(NetworkPlayer removedPlayer)
    {
        Debug.Log("Player left");

        int index = connectedPlayers.IndexOf(removedPlayer);

        if (index >= 0)
            connectedPlayers.RemoveAt(index);

        UpdatePlayersIDs();

        if (playerLeft != null)
            playerLeft(removedPlayer);

        if (removedPlayer != null)
            removedPlayer.choiceDone -= OnPlayerChoiceDone;
    }



    /// <summary>
    /// Called on the server when a player made his choice (in PFC)
    /// </summary>
    private void OnPlayerChoiceDone(NetworkPlayer player)
    {
        // on doit savoir si tous les joueurs ont choisi pour comparer les choix
    }


    protected void UpdatePlayersIDs()
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            connectedPlayers[i].SetPlayerID(i);
        }
    }




    /// <summary>
    /// Gets the NetworkPlayer object for a given connection
    /// </summary>
    public static NetworkPlayer GetPlayerForConnection(NetworkConnection conn)
    {
        return conn.playerControllers[0].gameObject.GetComponent<NetworkPlayer>();
    }

    /// <summary>
    /// Gets a network player by its index
    /// </summary>
    public NetworkPlayer GetPlayerById(int id)
    {
        return connectedPlayers[id];
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

    #region NetworkEvent



    public override void ServerChangeScene(string newSceneName)
    {
        Debug.Log("ServerChangeScene");

        base.ServerChangeScene(newSceneName);
    }



    public override void OnServerSceneChanged(string sceneName) // called automatically when ServerChangeScene loaded fully the scene
    {
        Debug.Log("OnServerSceneChanged");

        base.OnServerSceneChanged(sceneName);

        if (sceneChanged != null)
        {
            sceneChanged(true, sceneName);      // call event "sceneChanged" on both server and clients
        }



        //if (sceneChanged != null)               // Event call chez les clients pour afficher un message
        //{
        //    sceneChanged(true, sceneName);
        //}
    }





    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        // Intentionally not calling base here - we want to control the spawning of prefabs
        Debug.Log("OnServerAddPlayer");


        GameObject newPlayer = Instantiate<GameObject>(m_NetworkPlayerPrefab);
        DontDestroyOnLoad(newPlayer);
        NetworkServer.AddPlayerForConnection(conn, newPlayer.gameObject, playerControllerId); //associate prefab with his connection

    }






    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError");

        base.OnClientError(conn, errorCode);

        //if (clientError != null)                // Event call chez les clients pour afficher un message
        //{
        //    clientError(conn, errorCode);
        //}
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");

        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);

        //if (clientConnected != null)                // Event call chez les clients pour afficher un message
        //{
        //    clientConnected(conn);
        //}
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect");

        base.OnClientDisconnect(conn);

        //if (clientDisconnected != null)                // Event call chez les clients pour afficher un message
        //{
        //    clientDisconnected(conn);
        //}
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientDisconnect");

        base.OnClientDisconnect(conn);

        //if (serverError != null)                // Event call chez les clients pour afficher un message
        //{
        //    serverError(conn, errorCode);
        //}
    }



    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged");

        base.OnClientSceneChanged(conn);

        PlayerController pc = conn.playerControllers[0];   // pc = le 1er player sur cette connection 

        if (!pc.unetView.isLocalPlayer)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "PFCscene")
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                NetworkPlayer np = connectedPlayers[i];
                if (np != null)
                {
                    np.OnEnterPFCScene();
                }
            }
        }
        else if (sceneName == "MapRomainScene")
        {
            // Tell all network players that they're in the game scene
            for (int i = 0; i < connectedPlayers.Count; ++i)
            {
                NetworkPlayer np = connectedPlayers[i];
                if (np != null)
                {
                    np.OnEnterMapRomainScene();
                }
            }
        }
        if (sceneChanged != null)                       // call event "sceneChanged" on both server and clients
        {
            sceneChanged(false, sceneName);
        }
    }



    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Debug.Log("OnServerRemovePlayer");
        base.OnServerRemovePlayer(conn, player);

        NetworkPlayer connectedPlayer = GetPlayerForConnection(conn);
        if (connectedPlayer != null)
        {
            Destroy(connectedPlayer);
            connectedPlayers.Remove(connectedPlayer);
        }
    }


    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("OnServerReady");
        base.OnServerReady(conn);
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.LogFormat("OnServerConnect\nID {0}\nAddress {1}\nHostID {2}", conn.connectionId, conn.address, conn.hostId);

        if (numPlayers >= m_MultiplayerMaxPlayers)
        {
            conn.Disconnect();
        }

        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
        base.OnServerDisconnect(conn);
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        Debug.Log("OnMatchCreate");
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
        Debug.Log("OnMatchJoined");
    }

    public override void OnDropConnection(bool success, string extendedInfo)
    {
        base.OnDropConnection(success, extendedInfo);
        Debug.Log("OnDropConnection");

        //if (matchDropped != null)                // Event call chez les clients pour afficher un message
        //{
        //    matchDropped();
        //}
    }

    /// <summary>
    /// Server resets networkSceneName
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        networkSceneName = string.Empty;
    }

    /// <summary>
    /// Server destroys NetworkPlayer objects
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("OnStopServer");

        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            NetworkPlayer player = connectedPlayers[i];
            if (player != null)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

        connectedPlayers.Clear();

        // Reset this
        networkSceneName = string.Empty;
    }


    /// <summary>
    /// Clients also destroy their copies of NetworkPlayer
    /// </summary>
    public override void OnStopClient()
    {
        Debug.Log("OnStopClient");
        base.OnStopClient();

        for (int i = 0; i < connectedPlayers.Count; ++i)
        {
            NetworkPlayer player = connectedPlayers[i];
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }

        connectedPlayers.Clear();
    }

    /// <summary>
    /// Fire host started messages
    /// </summary>
    public override void OnStartHost()
    {
        Debug.Log("OnStartHost");
        base.OnStartHost();

        //if (m_NextHostStartedCallback != null)
        //{
        //    m_NextHostStartedCallback();
        //    m_NextHostStartedCallback = null;
        //}
        //if (hostStarted != null)
        //{
        //    hostStarted();
        //}
    }

    /// <summary>
    /// Called on the server when a player is set to ready
    /// </summary>
    //public virtual void OnPlayerSetReady(NetworkPlayer player)
    //{
    //    if (AllPlayersReady() && serverPlayersReadied != null)
    //    {
    //        serverPlayersReadied();
    //    }
    //}

    #endregion

}

