using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonSelectUnitScene : MonoBehaviour
{
    public SceneFader sceneFader;

    public void StartGame(string levelToGo)
    {
        if (AgentSelector.Instance.selectedAgents.Count == 3)
            sceneFader.FadeTo(levelToGo);
        else
            Debug.Log("Not enough agents selected");
    }
	
}
