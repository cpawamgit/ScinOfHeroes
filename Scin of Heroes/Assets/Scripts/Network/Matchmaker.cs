using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class Matchmaker : MonoBehaviour
{



    void Start()
    {
        NetworkManager.Instance.StartMatchMaker();
    }

    //call this method to request a match to be created on the server
    private void CreateInternetMatch()
    {
        NetworkManager.Instance.matchMaker.CreateMatch("", 2, true, "", "", "", 0, 0, OnInternetMatchCreate);
    }

    //this method is called when your request for creating a match is returned
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");

            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            NetworkManager.Instance.StartHost(hostInfo);


            ////////////////////////////////////////////////////////////////////////////////  // AFFICHER ECRAN CHARGEMENT
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    //call this method to find a match through the matchmaker
    public void FindInternetMatch()
    {
        NetworkManager.Instance.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnInternetMatchList);
    }

    //this method is called when a list of matches is returned
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                Debug.Log("A list of matches was returned");

                //join the last server (just in case there are two...)
                NetworkManager.Instance.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                CreateInternetMatch();
                Debug.Log("No matches in requested room!");
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Able to join a match");

            MatchInfo hostInfo = matchInfo;
            NetworkManager.Instance.StartClient(hostInfo);

            // ici nous avons 2 joueurs ready to play
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }
}