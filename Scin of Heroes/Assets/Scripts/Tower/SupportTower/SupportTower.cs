using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTower : Towers, IDamageable
{
    [Header("Healer")]
    public bool healer;
    public float healAmount;
    public float healRate;

    [Header("BoostMS")]
    public bool boostMS;
    public float speedModifier;

    [Header("BoostRes")]
    public bool boostRes;
    public float newRes;

    private bool waitForNextHeal = false;

    [Header("Effects")]
    public GameObject resEffect;


    protected override void OnEnable()
    {
        base.OnEnable();
        targetter.targetOutOfRange += LosingTarget;
    }

    protected override void OnDisable()
    {
        Debug.Log("OnDisable");
        foreach (GameObject ally in targetter.allies)
        {
            LosingTarget(ally);
        }
        base.OnDisable();
        targetter.targetOutOfRange -= LosingTarget;
    }

    private void Update()
    {
        if (targetter.allies.Count <= 0)
            return;

        if (healer && !waitForNextHeal)
        {
            StartCoroutine(Heal());
        }

        if (boostMS)
        {
            BoostMS();
        }
        if (boostRes)
        {
            BoostRes();
        }
    }

    private void BoostRes()
    {
        foreach (GameObject ally in targetter.allies)
        {
            if (ally.tag == "Tower" )
                continue;

            if (ally.GetComponent<ModifyRes>() == null)
            {
                ModifyRes modifyRes = ally.AddComponent<ModifyRes>();
                modifyRes.resModificator = newRes;
                modifyRes.resEffect = resEffect;
                modifyRes.ChangeRes();
            }
        }
    }

    IEnumerator Heal()
    {
        waitForNextHeal = true;

        foreach (GameObject ally in targetter.allies)
        {
            if (ally.tag == "Tower")
                continue;

            ally.GetComponent<IDamageable>().Heal(healAmount);
            ally.GetComponent<IDamageable>().TurnOnOffEffects("healEffect", true);
        }

        yield return new WaitForSeconds(healRate);

        waitForNextHeal = false;
    }

    private void BoostMS()
    {
        foreach (GameObject ally in targetter.allies)
        {
            if (ally.tag == "Tower")
                continue;

            ally.GetComponent<IDamageable>().ModifySpeed(speedModifier);
            ally.GetComponent<IDamageable>().TurnOnOffEffects("speedEffect", true);
        }
    }

    public void LosingTarget(GameObject target)
    {
        Debug.Log("LosingTarget");

        if (!targetter.allies.Contains(target))
            return;

        if (target.GetComponent<IDamageable>() == null)
            return;

        if (healer && target.tag != "Tower")
        {
            target.GetComponent<IDamageable>().TurnOnOffEffects("healEffect", false);
        }

        if (boostMS && target.tag != "Tower")
        {
            target.GetComponent<IDamageable>().ModifySpeed(1);
            target.GetComponent<IDamageable>().TurnOnOffEffects("speedEffect", false);
        }

        if (boostRes && target.tag != "Tower")
        {
            Destroy(target.GetComponent<ModifyRes>());

            Debug.Log("boostRes LosingTarget");
        }
    }

    protected override void Die()
    {
        foreach (GameObject ally in targetter.allies)
        {
            LosingTarget(ally);
        }
        base.Die();
    }

}
