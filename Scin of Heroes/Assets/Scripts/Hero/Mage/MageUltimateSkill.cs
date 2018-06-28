using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageUltimateSkill : Skills, IHeroUltimate
{


    public GameObject ultimateGO;

    private bool actif;
    private GameObject _ulti;


    protected override void Update ()
    {
        base.Update();

        if (skill == skill.inCD && actif)
        {
            actif = false;
            PoolManager.Instance.poolDictionnary[_ulti.name].UnSpawnObject(_ulti);
        }
	}


    public void ActiveUlti()
    {
        _ulti = PoolManager.Instance.poolDictionnary[ultimateGO.name].GetFromPool(transform.position);
        _ulti.GetComponent<Animator>().Play("MageUltimate");
        skill = skill.actif;
        actif = true;
    }


    


}
