using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Tourbilol : Skills
{
    [SerializeField]
    private float damageOverTime;
    [SerializeField]
    private float attackRate;


    


    protected override void OnEnable()
    {
        base.OnEnable();
        cdEnd += StopTourbilol;
        sphereTargetterAction += TourbilolDealDamage;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        cdEnd -= StopTourbilol;
        sphereTargetterAction -= TourbilolDealDamage;
    }

    protected override void Update()
    {
        base.Update();

        if (skill == skill.dispo && hero.targetter.targetEnemy != null)
        {
            skill = skill.actif;
            ActiveTourbilol();
        }
    }

    private void ActiveTourbilol()
    {
        hero.anim.Play("Tourbilol");
        InvokeRepeating("SphereTargetter", 0, attackRate);
    }

    private void TourbilolDealDamage()
    {
        foreach (IDamageable target in validTargets)
        {
            Debug.Log("TourbilolDealDamage");
            target.TakeDamage(damageOverTime);
        }
        validTargets.Clear();
    }


    private void StopTourbilol()
    {
        hero.anim.Play("Default");
        if (IsInvoking("SphereTargetter"))
            CancelInvoke("SphereTargetter");
    }
}
