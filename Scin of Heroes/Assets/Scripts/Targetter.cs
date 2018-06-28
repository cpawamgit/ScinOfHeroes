using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Targetter : MonoBehaviour
{
    public event Action<GameObject> targetOutOfRange;


    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public IDamageable targetEnemy;
    private SphereCollider sphereCollider;


    [Header("General")]

    public float range = 15f;
    public Alignement alignement;


    [Header("Unity Setup Fields")]


    public Transform partToRotate;
    public float turnSpeed = 10f;
    public Transform firePoint;
    public float scaleForGizmo;

    [HideInInspector]
    public List<GameObject> enemies;
    [HideInInspector]
    public List<GameObject> allies;


    private void OnEnable()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = range;
        enemies = new List<GameObject>();
        allies = new List<GameObject>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

   


   

    private void OnDisable()
    {
        CancelInvoke("UpdateTarget");
        enemies.Clear();
        allies.Clear();
        target = null;
        targetEnemy = null;
    }

    private void UpdateTarget()
    {
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].activeSelf)
                    enemies.RemoveAt(i);
            }

        }

        if (allies.Count > 0)
        {
            for (int i = 0; i < allies.Count; i++)
            {
                if (!allies[i].activeSelf)
                    allies.RemoveAt(i);
            }
        }


        if (target != null && enemies.Contains(target.gameObject))
        {
            return;
        }


        if (target != null)
        {
            if (!target.gameObject.activeSelf)
            {
                target = null;
                targetEnemy = null;
            }
        }

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;



        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<IDamageable>();
        }
        else
        {
            target = null;
        }

    }


    public void LockOnTarget()
    {
        Vector3 dir = (target.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

        if (collider.GetComponent<IDamageable>().GetAlignement() == alignement)
            allies.Add(collider.gameObject);
        else
            enemies.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IDamageable>() == null)
            return;

        if (targetOutOfRange != null)
            targetOutOfRange(other.gameObject);
       

        if (enemies.Contains(other.gameObject))
            enemies.Remove(other.gameObject);

        if (allies.Contains(other.gameObject))
            allies.Remove(other.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range*(scaleForGizmo / 1));
    }
}
