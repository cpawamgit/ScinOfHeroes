using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour {

    [Header("Use Laser")]


    public int damageOverTime = 30;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Setup")]
    public Targetter targetter;

    private void Update()
    {
        if (targetter.target == null)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                impactEffect.Stop();
                impactLight.enabled = false;
            }
            return;

        }
        targetter.LockOnTarget();
        Laser();
     }

    void Laser()
    {
        targetter.targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetter.targetEnemy.AddBuff("LaserSlow",slowAmount,BuffType.MSBoost);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, targetter.firePoint.position);
        lineRenderer.SetPosition(1, targetter.target.position);

        Vector3 dir = targetter.firePoint.position - targetter.target.position;

        impactEffect.transform.position = targetter.target.position + dir.normalized;

        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }
}
