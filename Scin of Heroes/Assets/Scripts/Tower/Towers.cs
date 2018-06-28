using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float startHealth = 100f;
    private float health;
    private bool isDead;

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

    /// <summary>
    /// Event that is fired when this instance is removed, such as when pooled or destroyed
    /// </summary>
    public event Action<GameObject> removed;


    protected virtual void OnEnable()
    {
        isDead = false;
        health = startHealth;
        towerHealthBar = GetComponentInChildren<EnemyHealthBar>();
        normalizedHealth = health / startHealth;
        towerHealthBar.UpdateEnnemyHealth(normalizedHealth);
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

    /// <summary>
    /// Return the Effect's Fx List that runs on this units
    /// </summary>
    public List<GameObject> GetEffectFxList()
    {
        Debug.LogWarning("Towers dont have EffectFxList, you dumbass !!!");
        return null;
    }


    protected virtual void Die()
    {
        isDead = true;

        if (removed != null)
        {
            removed(this.gameObject);
        }

        GameObject towerDeathEffectInst = MyObjectPooler.Instance.SpawnFromPool(towerDeathEffect);
        towerDeathEffectInst.transform.position = transform.position;
        towerDeathEffectInst.transform.rotation = transform.rotation;
        towerDeathEffectInst.SetActive(true);

        MyObjectPooler.Instance.ReturnToPool(gameObject);
    }


    #region IDamageable Methode Useless
    public void AddBuff(string buffName, float value, BuffType buffType)
    {
        Debug.LogWarning("Towers dont get Buffs, you dumbass !!!");
    }

    public void RemoveBuff(string buffName, float value, BuffType buffType)
    {
        Debug.LogWarning("Towers dont get Buffs, you dumbass !!!");
    }

    public Dictionary<string, List<float>> GetASBoostsDictionary()
    {
        Debug.LogWarning("Methode not supposed to be called on a tower");
        return null;
    }

    public Dictionary<string, List<float>> GetMSBoostsDictionary()
    {
        Debug.LogWarning("Methode not supposed to be called on a tower");
        return null;
    }

    public Dictionary<string, List<float>> GetResiBoostsDictionary()
    {
        Debug.LogWarning("Methode not supposed to be called on a tower");
        return null;
    }
    public void SetFx(GameObject fxPrefab)
    {
        Debug.LogWarning("Methode not supposed to be called on a tower");

    }
    public void RemoveFx(GameObject fxPrefab)
    {
        Debug.LogWarning("Methode not supposed to be called on a tower");
    }
    #endregion

}
