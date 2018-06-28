using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum skill
{
    inCD,
    actif,
    dispo,
}

[RequireComponent(typeof(Unite))]
public class Skills : MonoBehaviour
{
    protected UnityAction cdEnd;
    protected UnityAction sphereTargetterAction;

    [SerializeField]
    protected float baseDuration;      // how many seconds the skill last
    private float duration;         // how many seconds until the skill end
    [SerializeField]
    protected float baseCD;
    private float CD;
    public float returnCD { get { return CD; } }

    [SerializeField]
    protected float skillRange;

    public Hero hero;
    public Sprite UISprite;

    protected skill skill;
    public skill Skill { get { return skill; } }

    protected List<IDamageable> validTargets;


    protected virtual void OnEnable()
    {
        duration = baseDuration;
        CD = baseCD;
        skill = skill.dispo;
        validTargets = new List<IDamageable>();
    }



    protected virtual void OnDisable()
    {

    }


    protected virtual void Update()
    {
        if (skill == skill.actif)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                if (cdEnd != null)
                    cdEnd();

                skill = skill.inCD;
                duration = baseDuration;
            }
        }
        else if (skill == skill.inCD)
        {
            CD -= Time.deltaTime;
            if (CD <= 0)
            {
                skill = skill.dispo;
                CD = baseCD;
            }
        }

    }




    protected virtual void SphereTargetter()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, skillRange);

        foreach (Collider target in colliders)
        {
            if (target.gameObject.GetComponent<IDamageable>() == null)
                continue;
            if (target.gameObject.GetComponent<IDamageable>().GetAlignement() == null)
                continue;
            if (target.gameObject.GetComponent<IDamageable>().GetAlignement() != hero.GetAlignement())
                validTargets.Add(target.gameObject.GetComponent<IDamageable>());
        }

        if (sphereTargetterAction != null)
            sphereTargetterAction();
    }
}
