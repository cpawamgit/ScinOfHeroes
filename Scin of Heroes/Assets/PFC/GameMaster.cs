using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{

    #region Singleton
    public static GameMaster Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);

        Instance = this;
    }
    #endregion

    public UIPFCController UIPFCController;

    private List<NetworkPlayer> connectedPlayers;
    public List<NetworkPlayer> m_PlayersConnected
    { get { return connectedPlayers; } }

    private NetworkPlayer localPlayer;
    public NetworkPlayer m_LocalPlayer
    { get { return localPlayer; } }


    private NetworkPlayer otherPlayer;
    public NetworkPlayer m_OtherPlayer
    { get { return otherPlayer; } }





    void Start()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }

   


    public void SetPlayersConnectedList(List<NetworkPlayer> playersConnectedList)
    {
        Debug.Log("SetPlayersConnectedList");

        connectedPlayers = playersConnectedList;

        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].hasAuthority)
            {
                localPlayer = connectedPlayers[i];
                Debug.Log("localPlayer = " + localPlayer);
            }
            else
            {
                otherPlayer = connectedPlayers[i];
                Debug.Log("otherPlayer = " + otherPlayer);
            }
        }

        //UIPFCController.SetUI();
    }
}
