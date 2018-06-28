using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AgentSelector : MonoBehaviour
{

    #region Singleton
    public static AgentSelector Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion


    [System.Serializable]
    public class ButtonClass
    {
        public Image imageButton;
        //[HideInInspector]
        public bool clicked = false;
    }


    [HideInInspector]
    public List<GameObject> selectedAgents = new List<GameObject>();
    public List<string> selectedAgentsString = new List<string>();


    public List<ButtonClass> agentSelectButtons = new List<ButtonClass>();


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }



    public void SelectAgent(GameObject agentToSelect)
    {
        
        if (selectedAgents.Contains(agentToSelect))
        {
            selectedAgents.Remove(agentToSelect);
            selectedAgentsString.Remove(agentToSelect.name);
        }
        else if (selectedAgents.Count < 3)
        {
            selectedAgents.Add(agentToSelect);
            selectedAgentsString.Add(agentToSelect.name);
        }

    }




    public void ChangeButtonState(int button)
    {
        if (agentSelectButtons[button].clicked)
        {
            agentSelectButtons[button].imageButton.color = Color.white;
            agentSelectButtons[button].clicked = false;
        }
        else if(selectedAgents.Count < 3)
        {
            agentSelectButtons[button].imageButton.color = Color.red;
            agentSelectButtons[button].clicked = true;
        }

    }
}
