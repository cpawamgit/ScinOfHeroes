using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentSelectUiController : MonoBehaviour
{
    public Button startGameButton;

    private void Update()
    {
        if (AgentSelector.Instance.selectedAgents.Count >= 2)
        {
            startGameButton.interactable = true;
        }
        else 
        {
            startGameButton.interactable = false;
        }
    }

}
