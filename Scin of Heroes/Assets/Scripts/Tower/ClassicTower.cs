using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicTower : Towers, IDamageable
{


    [Header("Use Bullets (default)")]
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    private float fireCountdown = 0f;

    

    void Update()
    {
        if (targetter.target == null)
        {
            return;
        }


        targetter.LockOnTarget();

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f/fireRate;
            }

            fireCountdown -= Time.deltaTime;
    }




    void Shoot ()
    {
        GameObject bulletGO = MyObjectPooler.Instance.SpawnFromPool(bulletPrefab);
        bulletGO.transform.position = targetter.firePoint.position;
        bulletGO.transform.rotation = targetter.firePoint.rotation;
        bulletGO.SetActive(true);

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
            bullet.Seek(targetter.target);

    }
}
