using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float startHealth = 100f;
    protected float health;
    protected bool isDead;

    [HideInInspector]
    public float normalizedHealth;

    [Header("Effects")]
    public GameObject towerDeathEffect;
    //public GameObject speedEffect;
    //public GameObject healEffect;

    //private Dictionary<string, GameObject> effectDictionnary;
    private EnemyHealthBar towerHealthBar; // rename EnemyHealthBar to heatlhBar

    [Header("Setup")]
    public Targetter targetter;



    protected virtual void OnEnable()
    {
        isDead = false;
        health = startHealth;
        towerHealthBar = GetComponentInChildren<EnemyHealthBar>();
        normalizedHealth = health / startHealth;
        towerHealthBar.UpdateEnnemyHealth(normalizedHealth);

        //effectDictionnary = new Dictionary<string, GameObject>();
        //effectDictionnary.Add("speedEffect", speedEffect);
        //effectDictionnary.Add("healEffect", healEffect);

    }

    protected virtual void OnDisable()
    {

    }

    public Alignement GetAlignement()
    {
        return targetter.alignement;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        normalizedHealth = health / startHealth;
        towerHealthBar.UpdateEnnemyHealth(normalizedHealth); 

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

   
    public void Heal(float amount)
    {
        if (health + amount <= startHealth)
            health += amount;
        else
            health = startHealth;

        normalizedHealth = health / startHealth;
        towerHealthBar.UpdateEnnemyHealth(normalizedHealth);
    }

    public void TurnOnOffEffects(string effect, bool stateToTurn)
    {
        Debug.Log("Towers dont have speed, you dumbass !!!");
        return;

        //if (effectDictionnary[effect] == null)
        //{
        //    Debug.Log("No effect with name " + effect);
        //    return;
        //}

        //if (stateToTurn)
        //    effectDictionnary[effect].SetActive(true);
        //else
        //    effectDictionnary[effect].SetActive(false);

    }

    public void ModifySpeed(float pct)
    {
        Debug.Log("Towers dont have speed, you dumbass !!!");
        return;
    }


    protected virtual void Die()
    {
        isDead = true;

        GameObject towerDeathEffectInst = PoolManager.Instance.poolDictionnary[towerDeathEffect.name].GetFromPool(transform.position); 
        towerDeathEffectInst.transform.rotation = transform.rotation;

        PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
    }


    public void ChangeRes(float newRes, string modify)
    {
        Debug.Log("Tower dont have Res, at least for now");
    }
   
}
