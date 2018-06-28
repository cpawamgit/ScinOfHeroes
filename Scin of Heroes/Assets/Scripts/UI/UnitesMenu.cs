using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UnitesMenu : NetworkBehaviour
{
    public Vector3 spawnPoint;

    public List<Image> agentButtonsImageList;
    public List<Button> agentButtonsList;
    public List<Text> agentButtonsTextsList;

    private List<int> priceList = new List<int>(5);
    private List<float> CDList = new List<float>(5);
    private List<bool> onCDList = new List<bool>(5);

    private PoolManager poolManager;
    private AgentSelector agentSelector;


    private void Awake()
    {
        poolManager = PoolManager.Instance;
        agentSelector = AgentSelector.Instance;

        for (int i = 0; i < AgentSelector.Instance.selectedAgents.Count; i++)
        {
            agentButtonsImageList[i].sprite = AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().image;
            CDList.Add(AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().CDBetweenSpawns);
            priceList.Add(AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().price);
            onCDList.Add(false);
        }

    }


    private void Update()
    {
        int i = 0;

        foreach (Button button in agentButtonsList)
        {
            if (onCDList[i])
            {
                CDList[i] -= Time.deltaTime;
                agentButtonsTextsList[i].text = string.Format("{0:00.00}", CDList[i]);

                if (CDList[i] <= 0)
                {
                    onCDList[i] = false;
                    CDList[i] = AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().CDBetweenSpawns;
                    agentButtonsTextsList[i].text = null;
                    if (PlayerStats.Instance.Money > priceList[i])
                    {
                        button.interactable = true;
                    }
                }
            }
            i++;
        }
    }

    Vector3 test;

    public void SpawnUnite(int indexUniteToSpawn)
    {

        foreach (NetworkPlayer player in NetworkManager.Instance.connectedPlayers)
        {
            if (player.hasAuthority)
            {
                player.CmdSpawnFromPool(indexUniteToSpawn, test);
                GameObject unit = player.go;
            }
        }

        onCDList[indexUniteToSpawn] = true;
        agentButtonsList[indexUniteToSpawn].interactable = false;

        PlayerStats.Instance.ChangeMoney(-priceList[indexUniteToSpawn]);
    }

  

    public void CheckMoney()
    {
        int i = 0;

        foreach (Button button in agentButtonsList)
        {
            if (PlayerStats.Instance.Money < priceList[i])
            {
                if (button.interactable == true)
                    button.interactable = false;
            }
            else if (onCDList[i] == false)
            {
                if (button.interactable == false)
                    button.interactable = true;
            }

            i++;
        }
    }

}
