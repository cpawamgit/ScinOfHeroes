using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed;
    public float damage;
    public float explosionRadius;

    private Vector3 _targetPosition;
    private Alignement _sourceAlignement;
    private float step;

    private void Update()
    {
        step = speed * Time.deltaTime;
        //transform.Translate(_targetPosition * Time.deltaTime * speed);
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

        if (transform.position == _targetPosition)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider enemy in colliders)
            {
                if (enemy.gameObject.GetComponent<IDamageable>() == null)
                {
                    continue;
                }
                if (enemy.gameObject.GetComponent<IDamageable>().GetAlignement() == null)
                {
                    continue;
                }
                if (enemy.gameObject.GetComponent<IDamageable>().GetAlignement() == _sourceAlignement)
                {
                    continue;
                }

                enemy.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            }

            PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
        }

       
    }

    public void SetDestination(Vector3 targetPosition, Alignement sourceAlignement)
    {
        _targetPosition = targetPosition;
        _sourceAlignement = sourceAlignement;
    }
}
