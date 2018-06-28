using UnityEngine;
using System.Collections;



public class HeroMovement : MonoBehaviour
{

    public float shootDistance = 10f;
    //public float shootRate = .5f;
    //public HeroAttack attackingScript;

    private Animator anim;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Ray shootRay;
    private RaycastHit shootHit;
    private bool walking;

    private Transform targetedEnemy; //hero ou tour ennemie 
    private bool enemyClicked;
    private float nextFire;
    //private AttackAffector attackAffector;

    void Awake()
    {
        //anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //attackAffector = GetComponent<AttackAffector>();
    }


    void Update()
    {


        if (Input.GetButtonDown("Fire2"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast a ray from the camera to mouse cursor
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) //if we hit a collider
            {
                if (hit.collider.CompareTag("Hero") || hit.collider.CompareTag("Tower")) // ajouter alignement
                {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                }

                else
                {
                    walking = true;
                    enemyClicked = false;
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.isStopped = false;
                }
            }
        }

        if (enemyClicked)
        {
            MoveAndAttack();
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
                walking = false;
        }
        else
        {
            walking = true;
        }

        //anim.SetBool("IsWalking", walking);
    }

    private void MoveAndAttack()
    {
        if (targetedEnemy == null)
            return;
        navMeshAgent.destination = targetedEnemy.position;
        if (navMeshAgent.remainingDistance >= shootDistance) //tant que la cible est plus loin que la range d attaque, continuer de se déplacer + walking anim
        {
            navMeshAgent.isStopped = false;
            walking = true;
        }

        if (navMeshAgent.remainingDistance <= shootDistance)
        {
            transform.LookAt(targetedEnemy);
            navMeshAgent.isStopped = true;
        }




    }
}

