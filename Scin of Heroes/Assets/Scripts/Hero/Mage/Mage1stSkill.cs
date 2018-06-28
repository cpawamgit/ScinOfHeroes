using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage1stSkill : Skills {

    public UltimateTargetter ultimateTargetter;
    public float damage;
    public GameObject effect;


    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

        if (skill == skill.dispo && ultimateTargetter.enemies.Count > 0)
        {
            skill = skill.actif;
            ActiveSpell();
        }
	}


    private void ActiveSpell()
    {
        foreach (GameObject enemy in ultimateTargetter.enemies)
        {
            enemy.GetComponent<IDamageable>().TakeDamage(damage);
        }
        effect.SetActive(true);
        StartCoroutine(TurnOffEffect());

        skill = skill.inCD;
    }

    IEnumerator TurnOffEffect()
    {
        yield return new WaitForSeconds(0.5f);

        effect.SetActive(false);
    }
}