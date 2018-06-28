using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUI : MonoBehaviour
{


    #region Singleton

    public static HeroUI Instance;

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

    #endregion


    public List<Image> images;
    public List<Text> texts;
    public Button ultimateButton;

    [HideInInspector]
    public Hero hero;

    private Skills ultimate;

    public void Init()
    {
        for (int i = 0; i < hero.skills.Count; i++)
        {
            images[i].sprite = hero.skills[i].UISprite;
            texts[i].text = "";
        }

        if (hero.gameObject.GetComponent<IHeroUltimate>() != null)
        {
            ultimateButton.onClick.RemoveAllListeners();
            ultimateButton.onClick.AddListener(hero.gameObject.GetComponent<IHeroUltimate>().ActiveUlti);
            ultimate = hero.skills[3];
        }
    }




    private void Update()
    {
        Debug.Log("ultimate.Skill = " + ultimate.Skill);

        for (int i = 0; i < hero.skills.Count; i++)
        {
            if (hero.skills[i].Skill == skill.inCD)
            {
                texts[i].text = string.Format("{0:00.00}", hero.skills[i].returnCD);
            }
        }

        if (ultimateButton.interactable == true && ultimate.Skill != skill.dispo)
        {
            ultimateButton.interactable = false;
        }


        if (ultimateButton.interactable == false && ultimate.Skill == skill.dispo)
        {
            ultimateButton.interactable = true;
        }
    }
}
