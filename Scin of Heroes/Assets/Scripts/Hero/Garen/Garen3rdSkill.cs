using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garen3rdSkill : Skills   // actualy this skill is only for melee, look below to add range
{
    private int attackNumber;
    public int boostDamage; // the third attack will deal base AAdamage + boostDamage

    protected override void OnEnable()
    {
        base.OnEnable();
        hero.attackAction += BoostThirdAttack;
        attackNumber = 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        hero.attackAction -= BoostThirdAttack;
    }


    private void BoostThirdAttack()
    {
        attackNumber++;

        Debug.Log("BoostThirdAttack");

        if (attackNumber >= 3)
        {
            Debug.Log("attackNumber = "+ attackNumber);

            hero.targetter.targetEnemy.TakeDamage(100);      // work for melee, if this class is needed for range, we need to check if the unite is range or melee and describe differents methods here

            attackNumber = 0;
        }
    }

}
