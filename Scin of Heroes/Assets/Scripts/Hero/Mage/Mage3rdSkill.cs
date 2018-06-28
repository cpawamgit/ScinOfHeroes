using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage3rdSkill : Skills
{
    public float m_ExplosionForce;
    public float m_ExplosionRadius;
    public GameObject skillEffect;
	
	protected override void Update ()
    {
        base.Update();

        if (skill == skill.dispo && hero.Health <= hero.startHealth/3)
        {
            ActiveSkill();
        }
	}


    private void ActiveSkill()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);

        foreach (Collider enemy in colliders)
        {
            if (enemy.gameObject.GetComponent<IDamageable>() == null)
            {
                continue;
            }
            if (enemy.gameObject.GetComponent<IDamageable>().GetAlignement() == null)
            {
                continue;
            }
            if (enemy.gameObject.GetComponent<IDamageable>().GetAlignement() == hero.GetAlignement())
            {
                continue;
            }


            Rigidbody targetRigidbody = enemy.GetComponent< Rigidbody>();

            if (!targetRigidbody)
                continue;


            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
        }

            skill = skill.inCD;
    }

}
