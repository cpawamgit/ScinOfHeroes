using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;




public class Unite : MonoBehaviour, IDamageable
{
    public UnityAction takeDamageAction;
    public UnityAction attackAction;

    [Header("Stats")]
    public float fireRate = 1f;
    public GameObject bulletPrefab; // must be ignored if melee ,  faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool
    public int value = 50;
    public float CDBetweenSpawns;
    public int price;
    [SerializeField]
    private float baseResistance = 0f;
    public float BaseResistance
    { get { return baseResistance; } }
    private float resistance;
    public float Resistance
    { get { return resistance; } }

    [SerializeField]
    public bool range = false;
    public bool melee = false;

    public int AAdamage; //auto attack damage, must be ignored if range , faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool

    public float startSpeed = 10f;
    public float startHealth = 100f;
    [HideInInspector]
    public float normalizedHealth;

    [Header("Effects")]
    public Sprite image;
    public GameObject speedEffect;
    public GameObject healEffect;
    public GameObject resEffect;
    public GameObject deathEffect;

    [Header("Setup")]
    public Targetter targetter;
    public UnitData unitData;


    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Animator anim;


    protected bool isDead = false;
    private float fireCountdown = 0f;
    private float health;
    public float Health
    { get { return health; } }

   

    private EnemyHealthBar enemyHealthBar;


    private Dictionary<string, GameObject> effectDictionnary;



    protected virtual void Awake()
    {

    }
   

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        resistance = baseResistance;
        speed = startSpeed;
        health = startHealth;
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        isDead = false;
        normalizedHealth = health / startHealth;
        enemyHealthBar.UpdateEnnemyHealth(normalizedHealth);

        effectDictionnary = new Dictionary<string, GameObject>();
        effectDictionnary.Add("speedEffect", speedEffect);
        effectDictionnary.Add("healEffect", healEffect);
        effectDictionnary.Add("resEffect", resEffect);
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
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }




    protected  virtual void Attack()
    {
        if (attackAction != null)
        {
            Debug.Log("fire attackAction");
            attackAction();
        }


        if (range)
        {
            GameObject bulletGO = PoolManager.Instance.poolDictionnary["Bullet"].GetFromPool(targetter.firePoint.position);

            bulletGO.transform.rotation = targetter.firePoint.rotation;

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

        float effectivDamage = amount * (1 - resistance);

        health -= effectivDamage;
        normalizedHealth = health / startHealth;
        enemyHealthBar.UpdateEnnemyHealth(normalizedHealth);

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    public void ModifySpeed (float multiplier)
    {
        speed = startSpeed * multiplier;
    }

    virtual protected void Die()
    {
        isDead = true;

        GameObject deathEffectInst = PoolManager.Instance.poolDictionnary[deathEffect.name].GetFromPool(transform.position); 
        deathEffectInst.transform.rotation = transform.rotation;

        PlayerStats.Instance.ChangeMoney(value);

        WaveSpawner.EnemiesAlive--;

        PoolManager.Instance.poolDictionnary[deathEffect.name].UnSpawnObject(gameObject);
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

    public void TurnOnOffEffects(string effect, bool stateToTurn)
    {
        if (effectDictionnary[effect] == null)
        {
            Debug.Log("No effect with name " + effect);
            return;
        }

        if (stateToTurn)
            effectDictionnary[effect].SetActive(true);
        else
            effectDictionnary[effect].SetActive(false);

    }


    List<float> resList = new List<float>();


    private void AddRes(float newRes)
    {
        resList.Add(newRes);
        ActualiseRes();
    }

    private void RemoveRes(float newRes)
    {
        resList.Remove(newRes);
        ActualiseRes();
    }

    private void ActualiseRes()
    {
        resistance = baseResistance;

        if (resList.Count == 0)
        {
            resistance = baseResistance;
        }
        foreach (float res in resList)
        {
            resistance += (1 - resistance) / (1 / res);
        }
    }

    public void ChangeRes(float newRes, string modify)
    {
        if (modify == "add")
            AddRes(newRes);
        else if (modify == "remove")
            RemoveRes(newRes);
        else
            Debug.Log("wrong string");
    }



}
