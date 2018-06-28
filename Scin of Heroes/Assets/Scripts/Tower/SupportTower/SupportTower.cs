using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*//////////////////////////////////////////////////////////
 
     
     HEY IF YOU SEE THAT THERE IS STILL WORK TO DO, CHECK BELLOW!

    -For Effect applie by buff you must get them from pool cuz there r instantiate for the moment
    -Clean the mess and add summaries on you methodes and variables
    -Check if all team's members are okay with the Heal Coroutine( instant heal of wait healRate)
    -Back to pool pour les effet de RemoveFx()
     
     ps: please
     
 /////////////////////////////////////////////////////////*/
public class SupportTower : Towers, IDamageable
{
    /// <summary>
    /// This enum is used when a specific tower buff is set for a unit, to check if we Buffing it or remove it from
    /// </summary>
    public enum BuffStat
    {
        Buff,
        DeBuff
    }

    [Header("Healer")]
    public bool healer;
    public float healAmount;
    public float healRate;
    public GameObject healFxPrefab;

    [Header("BoostAS")]
    public bool isAttackSpeedBooster;
    [Range(1, 10)]
    public float aSModifier = 1f;
    public GameObject attackSpeedFxPrefab;

    [Header("BoostMS")]
    public bool isMouvmentSpeedBooster;
    [Range(1, 10)]
    public float mSModifier = 1f;
    public GameObject movementSpeedFxPrefab;

    [Header("BoostRes")]
    public bool isResistanceBooster;
    [Range(0, 1)]
    public float resModifier;
    public GameObject resistanceFxPrefab;

    [Header("")]
    /// <summary>
    /// The particle system that plays when an entity enters the sphere
    /// </summary>
    public ParticleSystem enterParticleSystem;

    /// <summary>
    /// The audio source that plays when an entity enters the sphere
    /// </summary>
    public AudioSource enterAudioSource;

    /// <summary>
    /// List used to keep track off allies in Range
    /// </summary>
    private List<GameObject> alliesInRange = new List<GameObject>();

    protected override void OnEnable()
    {
        base.OnEnable();
        targetter.allieEnterRange += OnFriendEnterRange;
        targetter.allieExitRange += OnFriendExitRange;
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
        targetter.allieEnterRange -= OnFriendEnterRange;
        targetter.allieExitRange -= OnFriendExitRange;

        foreach (GameObject ally in alliesInRange)
        {
            RemoveAllBuffs(ally);
        }

        alliesInRange.Clear();

        base.OnDisable();
    }

    /// <summary>
    /// Fonction called when targetter spot an allie and trigger the allieEnterRange event
    /// We check if the gameobject is nothing but and Unit/Hero/Peon then it's time to support
    /// </summary>
    /// <param name="allie">GameObject that enter the targetter's range</param>
    private void OnFriendEnterRange(GameObject allie)
    {
        if (allie.tag == "Tower")
        {
            return;
        }

        IDamageable m_UniteDamageable = allie.GetComponent<IDamageable>();

        if (m_UniteDamageable == null)
        {
            return;
        }

        alliesInRange.Add(allie);
        m_UniteDamageable.removed += RemoveAllBuffs;

        //play sound/particle effect on enter range
        if (enterParticleSystem != null)
        {
            enterParticleSystem.Play();
        }
        if (enterAudioSource != null)
        {
            enterAudioSource.Play();
        }

        //Check what this tower is capable of
        if (healer && alliesInRange.Count <= 1)
        {
            StartCoroutine(Heal());
            m_UniteDamageable.SetFx(healFxPrefab);
        }
        if (isMouvmentSpeedBooster)
        {
            SetMsBuff(m_UniteDamageable, BuffStat.Buff);
            m_UniteDamageable.SetFx(movementSpeedFxPrefab);
        }
        if (isResistanceBooster)
        {
            SetResiBuff(m_UniteDamageable, BuffStat.Buff);
            m_UniteDamageable.SetFx(resistanceFxPrefab);
        }
        if (isAttackSpeedBooster)
        {
            SetASBuff(m_UniteDamageable, BuffStat.Buff);
            m_UniteDamageable.SetFx(attackSpeedFxPrefab);
        }

    }

    /// <summary>
    /// Called by the targetter event when and allie exits his range
    /// </summary>
    /// <param name="allie">the allie who has exit the range</param>
    private void OnFriendExitRange(GameObject allie)
    {
        alliesInRange.Remove(allie);
        RemoveAllBuffs(allie);
    }

    /// <summary>
    /// Removes all the buff and the ally
    /// </summary>
    /// <param name="allie">the allie who is target by this methode</param>
    private void RemoveAllBuffs(GameObject allie)
    {
        if (allie.tag == "Tower")
        {
            return;
        }

        IDamageable m_UniteDamageable = allie.GetComponent<IDamageable>();

        if (m_UniteDamageable == null)
        {
            return;
        }

        m_UniteDamageable.removed -= RemoveAllBuffs;

        if (healer)
        {
            if (alliesInRange.Count == 0)
            {
                StopAllCoroutines();
            }
            m_UniteDamageable.RemoveFx(healFxPrefab);
        }
        if (isMouvmentSpeedBooster)
        {
            SetMsBuff(m_UniteDamageable, BuffStat.DeBuff);
            if (!m_UniteDamageable.GetMSBoostsDictionary().ContainsKey("SupportTowerBuffMS"))
            {
                m_UniteDamageable.RemoveFx(movementSpeedFxPrefab);
            }
        }
        if (isResistanceBooster)
        {
            SetResiBuff(m_UniteDamageable, BuffStat.DeBuff);
            if (!m_UniteDamageable.GetResiBoostsDictionary().ContainsKey("SupportTowerBuffResi"))
            {
                m_UniteDamageable.RemoveFx(resistanceFxPrefab);
            }
        }
        if (isAttackSpeedBooster)
        {
            SetASBuff(m_UniteDamageable, BuffStat.DeBuff);
            if (!m_UniteDamageable.GetASBoostsDictionary().ContainsKey("SupportTowerBuffAS"))
            {
                m_UniteDamageable.RemoveFx(attackSpeedFxPrefab);
            }
        }
    }

    /// <summary>
    /// Simple Coroutine that keep heal allies in tower's range
    /// </summary>
    IEnumerator Heal()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(healRate);

            foreach (GameObject ally in targetter.allies)
            {
                if (ally.tag == "Tower")
                    continue;
                IDamageable allieIDamageable = GetComponent<IDamageable>();
                allieIDamageable.Heal(healAmount);
            }
        }
    }

    /// <summary>
    /// Fonction called to boost Resistance of an target allie
    /// </summary>
    /// <param name="UniteDamageable">The allie target we'r boosting</param>
    private void SetResiBuff(IDamageable uniteDamageable, BuffStat state)
    {
        if (uniteDamageable == null)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    uniteDamageable.AddBuff("SupportTowerBuffResi", resModifier, BuffType.ResistanceBoost);
                }
                break;
            case BuffStat.DeBuff:
                {
                    uniteDamageable.RemoveBuff("SupportTowerBuffResi", resModifier, BuffType.ResistanceBoost);
                }
                break;
        }
    }

    /// <summary>
    /// Fonction called to boost MS of an target allie
    /// </summary>
    /// <param name="UniteDamageable">The allie target we'r boosting</param>
    private void SetMsBuff(IDamageable uniteDamageable, BuffStat state)
    {
        if (uniteDamageable == null)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    uniteDamageable.AddBuff("SupportTowerBuffMS", mSModifier, BuffType.MSBoost);
                }
                break;
            case BuffStat.DeBuff:
                {
                    uniteDamageable.RemoveBuff("SupportTowerBuffMS", mSModifier, BuffType.MSBoost);
                }
                break;
        }
    }

    /// <summary>
    /// Called to Buff/DeBuff an unit
    /// </summary>
    /// <param name="uniteDamageable">unit target by the methode</param>
    /// <param name="state">use to check if its a buff or a debuff</param>
    private void SetASBuff(IDamageable uniteDamageable, BuffStat state)
    {
        if (uniteDamageable == null)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    uniteDamageable.AddBuff("SupportTowerBuffAS", aSModifier, BuffType.ASBoost);

                }
                break;
            case BuffStat.DeBuff:
                {
                    uniteDamageable.RemoveBuff("SupportTowerBuffAS", aSModifier, BuffType.ASBoost);
                }
                break;
        }
    }

    protected override void Die()
    {
        base.Die();
    }
}
