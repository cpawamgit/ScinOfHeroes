using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

/// <summary>
/// Type of the Buff can be
/// </summary>
public enum BuffType
{
    ResistanceBoost,
    ASBoost,
    MSBoost
}

public class Unite : MonoBehaviour, IDamageable
{
    #region Variables

    private float fireCountdown = 0f;
    private float health;

    private bool isDead = false;

    private EnemyHealthBar enemyHealthBar;
    private Animator m_Anim;

    /// <summary>
    /// Contains this unit's AsBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> aSBoostsDictionary;
    /// <summary>
    /// Contains this unit's MSBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> mSBoostsDictionary;
    /// <summary>
    /// Contains this unit's ResistanceBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> resiBoostsDictionary;

    /// <summary>
    /// List use to keep track of all Fx's Effect on this unit
    /// </summary>
    private List<GameObject> effectFxList;

    /// <summary>
    /// Event that is fired when this instance is removed, such as when pooled or destroyed
    /// </summary>
    public event Action<GameObject> removed;

    public UnityAction takeDamageAction;
    public UnityAction attackAction;

    [Header("Stats")]
    public float m_FireRate = 1f;
    public GameObject bulletPrefab; // must be ignored if melee ,  faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool
    public int value = 50;
    public float CDBetweenSpawns;
    public int price;
    [SerializeField]
    private float m_MovementSpeed = 10f;
    [SerializeField]
    private float m_Resistance = 0f;

    [SerializeField]
    public bool range = false;
    public bool melee = false;

    public int AAdamage; //auto attack damage, must be ignored if range , faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool

    public float startHealth = 100f;
    [HideInInspector]
    public float normalizedHealth;

    [Header("Setup")]
    public Targetter targetter;
    public Sprite icon;

    /// <summary>
    /// Position offset for an applied affect
    /// </summary>
    public Vector3 appliedEffectOffset = Vector3.zero;

    /// <summary>
    /// Scale adjustment for an applied affect
    /// </summary>
    public float appliedEffectScale = 1;

    public GameObject deathEffect;

    #endregion

    public Animator anim
    {
        get { return anim; }
    }

    #region Proprities

    /// <summary>
    /// Gets this unit's original Resistance
    /// </summary>
    public float baseResistance { get; private set; }

    /// <summary>
    /// Gets this unit's original movement speed
    /// </summary>
    public float baseMovementSpeed { get; private set; }

    /// <summary>
    /// Gets this unit's movement speed
    /// </summary>
    public float movementSpeed
    {
        get { return m_MovementSpeed; }
    }

    /// <summary>
    /// Gets this unit's original fire rate
    /// </summary>
    public float basefireRate { get; private set; }


    /// <summary>
    /// Return position offset for an applied affect
    /// </summary>
    public Vector3 GetEffectOffset()
    {
        return appliedEffectOffset;
    }

    /// <summary>
    /// Return scale adjustment for an applied affect
    /// </summary>
    public float GetEffectScale()
    {
        return appliedEffectScale;
    }

    /// <summary>
    /// Return the Effect's Fx List that runs on this units
    /// </summary>
    public List<GameObject> GetEffectFxList()
    {
        return effectFxList;
    }

    /// <summary>
    /// Return the Dictionary of Attack Speed's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetASBoostsDictionary()
    {
        return aSBoostsDictionary;
    }
    /// <summary>
    /// Return the Dictionary of Movement Speed's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetMSBoostsDictionary()
    {
        return mSBoostsDictionary;
    }
    /// <summary>
    /// Return the Dictionary of Resistance's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetResiBoostsDictionary()
    {
        return resiBoostsDictionary;
    }
    #endregion
    
    private void Awake()
    {
        LazyLoad();
    }

    /// <summary>
    /// This is a lazy way of caching several components utilised by the unit
    /// </summary>
    protected virtual void LazyLoad()
    {
        baseMovementSpeed = m_MovementSpeed;
        baseResistance = m_Resistance;
        basefireRate = m_FireRate;
        aSBoostsDictionary = new Dictionary<string, List<float>>();
        mSBoostsDictionary = new Dictionary<string, List<float>>();
        resiBoostsDictionary = new Dictionary<string, List<float>>();
        effectFxList = new List<GameObject>();
    }

    private void OnEnable()
    {
        m_Anim = GetComponent<Animator>();
        LazyLoad();
        health = startHealth;
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        isDead = false;
        normalizedHealth = health / startHealth;
        enemyHealthBar.UpdateEnnemyHealth(normalizedHealth);
    }


    protected virtual void Update()
    {
        if (targetter.target == null)
        {
            return;
        }

        targetter.LockOnTarget();

        if (fireCountdown <= 0f)
        {
            Attack();
            fireCountdown = 1f / m_FireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    protected virtual void Attack()
    {
        if (attackAction != null)
        {
            Debug.Log("fire attackAction");
            attackAction();
        }

        if (range)
        {
            GameObject bulletGO = MyObjectPooler.Instance.SpawnFromPool(bulletPrefab);
            bulletGO.transform.position = targetter.firePoint.position;
            bulletGO.transform.rotation = targetter.firePoint.rotation;
            bulletGO.SetActive(true);

            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Seek(targetter.target);
        }

        if (melee)
        {
            targetter.targetEnemy.TakeDamage(AAdamage);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (takeDamageAction != null)
        {
            takeDamageAction();
        }

        float effectivDamage = amount * (1 - m_Resistance);

        health -= effectivDamage;
        normalizedHealth = health / startHealth;
        enemyHealthBar.UpdateEnnemyHealth(normalizedHealth);

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    #region Buff & Fx
    /// <summary>
    /// Called when a buff must be add. Tower, spells and summoners can use this methode
    /// </summary>
    /// <param name="buffName">The name of the Buff we will Add</param>
    /// <param name="value">The value used to change stats</param>
    public void AddBuff(string buffName, float value, BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (aSBoostsDictionary.ContainsKey(buffName))
                    {
                        aSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        aSBoostsDictionary.Add(buffName, buffNameList);
                        aSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (mSBoostsDictionary.ContainsKey(buffName))
                    {
                        mSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        mSBoostsDictionary.Add(buffName, buffNameList);
                        mSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (resiBoostsDictionary.ContainsKey(buffName))
                    {
                        resiBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        resiBoostsDictionary.Add(buffName, buffNameList);
                        resiBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Called when a buff msut be removed. Tower, spells and summoners can use this methode
    /// </summary>
    /// <param name="buffName">The name of the Buff we will Remove</param>
    /// <param name="value">The value used to change stats</param>
    public void RemoveBuff(string buffName, float value, BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (aSBoostsDictionary.ContainsKey(buffName))
                    {
                        aSBoostsDictionary[buffName].Remove(value);
                        if (aSBoostsDictionary[buffName].Count == 0)
                        {
                            aSBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (mSBoostsDictionary.ContainsKey(buffName))
                    {
                        mSBoostsDictionary[buffName].Remove(value);
                        if (mSBoostsDictionary[buffName].Count == 0)
                        {
                            mSBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (resiBoostsDictionary.ContainsKey(buffName))
                    {
                        resiBoostsDictionary[buffName].Remove(value);
                        if (resiBoostsDictionary[buffName].Count == 0)
                        {
                            resiBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Will actualise the stats of the unit regarding his buffs
    /// </summary>
    private void ActualiseBuffs(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(aSBoostsDictionary))
                    {
                        m_FireRate = basefireRate;
                    }
                    else
                    {
                        float m_ASBuffs = 0;
                        foreach (KeyValuePair<string, List<float>> aSbuff in aSBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in aSbuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_ASBuffs += max;
                        }
                        m_FireRate = basefireRate * m_ASBuffs;

                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(mSBoostsDictionary))
                    {
                        m_MovementSpeed = baseMovementSpeed;
                    }
                    else
                    {
                        float m_MSBuffs = 0;
                        foreach (KeyValuePair<string, List<float>> mSbuff in mSBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in mSbuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_MSBuffs += max;
                        }
                        m_MovementSpeed = baseMovementSpeed * m_MSBuffs;

                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(resiBoostsDictionary))
                    {
                        m_Resistance = baseResistance;
                    }
                    else
                    {
                        float m_ResBuffs = baseResistance;
                        foreach (KeyValuePair<string, List<float>> resBuff in resiBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in resBuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_ResBuffs += (1 - m_ResBuffs) / (1 / max);
                        }
                        m_Resistance = m_ResBuffs;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Check a dictionary's key if there value contains an empty list
    /// </summary>
    /// <param name="dictionary">Dictionary to check</param>
    private bool SearchAndDeleteEmptyEntry(Dictionary<string, List<float>> dictionary)
    {
        if (dictionary.Count == 0)
        {
            return false;
        }
        else
        {
            List<string> itemsToRemove = new List<string>();

            foreach (var pair in dictionary)
            {
                if (pair.Value.Count.Equals(0))
                    itemsToRemove.Add(pair.Key);
            }

            foreach (string item in itemsToRemove)
            {
                dictionary.Remove(item);
            }
            return true;
        }
    }

    /// <summary>
    /// Add a visual for the buff/debuff that is affecting the allie
    /// </summary>
    /// <param name="allie">allie affect by the visual</param>
    /// <param name="fxPrefab">visual to set</param>
    /// <param name="position">position on the allie for the visual</param>
    /// <param name="scale">scale of the visual</param>
    public void SetFx(GameObject fxPrefab = null)
    {
        if ((!effectFxList.Contains(fxPrefab)) && fxPrefab != null)
        {
            GameObject m_fxPrefab = Instantiate(fxPrefab);
            m_fxPrefab.name = fxPrefab.name;
            effectFxList.Add(m_fxPrefab);
            m_fxPrefab.transform.parent = transform;
            m_fxPrefab.transform.localPosition = appliedEffectOffset;
            m_fxPrefab.transform.localScale *= appliedEffectScale;
        }
    }

    /// <summary>
    /// Remove a visual for the buff/debuff that is affecting the allie
    /// </summary>
    /// <param name="allie">allie affect by the visual</param>
    /// <param name="fxPrefab">visual to set</param>
    /// <param name="position">position on the allie for the visual</param>
    /// <param name="scale">scale of the visual</param>
    public void RemoveFx(GameObject fxPrefab = null)
    {
        if (fxPrefab != null)
        {
            for (int i = 0; i < effectFxList.Count; i++)
            {
                if (fxPrefab.name.GetHashCode() == effectFxList[i].name.GetHashCode())
                {
                    GameObject m_fxPrefab = effectFxList[i];

                    effectFxList.Remove(effectFxList[i]);

                    m_fxPrefab.transform.localScale = Vector3.one;
                    Destroy(m_fxPrefab);
                }
            }
        }
    }

    #endregion

    void Die()
    {
        isDead = true;

        if (removed != null)
        {
            removed(this.gameObject);
        }

        aSBoostsDictionary = null;
        resiBoostsDictionary = null;
        mSBoostsDictionary = null;
        effectFxList = null;

        GameObject deathEffectInst = MyObjectPooler.Instance.SpawnFromPool(deathEffect);
        deathEffectInst.transform.position = transform.position;
        deathEffectInst.transform.rotation = transform.rotation;
        deathEffectInst.SetActive(true);

        PlayerStats.Instance.ChangeMoney(value);

        WaveSpawner.EnemiesAlive--;

        MyObjectPooler.Instance.ReturnToPool(gameObject);
    }

    public Alignement GetAlignement()
    {
        return targetter.alignement;
    }

    public void Heal(float amount)
    {
        if (health + amount <= startHealth)
            health += amount;
        else
            health = startHealth;

        normalizedHealth = health / startHealth;
        enemyHealthBar.UpdateEnnemyHealth(normalizedHealth);
    }
}
