using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateTargetter : MonoBehaviour {

    [HideInInspector]
    public List<GameObject> enemies;
    private List<IDamageable> enemiesToDamage;
    public Alignement alignement;

    private void OnEnable()
    {
        enemies = new List<GameObject>();
    }

    private void Update()
    {
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].activeSelf)
                    enemies.RemoveAt(i);
            }

        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<IDamageable>() == null)
            return;

        if (collider.GetComponent<IDamageable>().GetAlignement() == null)
        {
            Debug.Log("Target dont have an alignement.");
            return;
        }

        if (!(collider.GetComponent<IDamageable>().GetAlignement() == alignement))
            enemies.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IDamageable>() == null)
            return;

        if (enemies.Contains(other.gameObject))
            enemies.Remove(other.gameObject);
    }
}
