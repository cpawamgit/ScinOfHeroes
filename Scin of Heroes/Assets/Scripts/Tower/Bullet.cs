using UnityEngine;

public class Bullet : MonoBehaviour {

    private Transform target;

    public float speed = 70f;
    public int damage = 50;

    public float explosionRadius = 0f;

    public GameObject impactEffect;

    public void Seek (Transform _target)
    {
        target = _target;
    }
	void Update () {
		if (target == null)
        {
            MyObjectPooler.Instance.ReturnToPool(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
	}

    void HitTarget()
    {
        GameObject impactEffectInst = MyObjectPooler.Instance.SpawnFromPool(impactEffect);
        impactEffectInst.transform.position = transform.position;
        impactEffectInst.transform.rotation = transform.rotation;
        impactEffectInst.SetActive(true);

        if (explosionRadius > 0f)
        {
            Explode();
        }else
        {
            Damage(target);
        }

        MyObjectPooler.Instance.ReturnToPool(gameObject);

    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }

    }

    void Damage (Transform enemy)
    {
        IDamageable e = enemy.GetComponent<IDamageable>();

        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
