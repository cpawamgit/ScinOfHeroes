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
            PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
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
        GameObject impactEffectInst = PoolManager.Instance.poolDictionnary[impactEffect.name].GetFromPool(transform.position); 
        impactEffectInst.transform.rotation = transform.rotation;

        if (explosionRadius > 0f)
        {
            Explode();
        }else
        {
            Damage(target);
        }

        PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
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
