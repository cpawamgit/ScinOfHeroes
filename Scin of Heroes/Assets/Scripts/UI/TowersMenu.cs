using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowersMenu : MonoBehaviour
{
    public List<TurretBlueprint> bluePrintList;

    public GameObject arrow;
    public GameObject inverseArrow;
    public Animator anim;

    public List<Button> buttonList;

    BuildManager buildManager;


    void Start()
    {
        buildManager = BuildManager.Instance;
    }

    public void SelectTurretToBuild(int index)
    {
        Debug.Log("Standard Turret Selected");
        buildManager.SelectTurretToBuild(bluePrintList[index]);
    }


    public void OpenMenu()
    {
        anim.SetTrigger("OpenTowersMenu");
        arrow.SetActive(false);
        inverseArrow.SetActive(true);
    }


    public void CloseMenu()
    {
        anim.SetTrigger("CloseTowersMenu");
        arrow.SetActive(true);
        inverseArrow.SetActive(false);
    }

    public void CheckMoney()
    {
        int i = 0;

        foreach (Button button in buttonList)
        {
            if (PlayerStats.Instance.Money < bluePrintList[i].cost)
            {
                if (button.interactable == true)
                    button.interactable = false;
            }
            else 
            {
                if (button.interactable == false)
                    button.interactable = true;
            }

            i++;
        }
    }
}
