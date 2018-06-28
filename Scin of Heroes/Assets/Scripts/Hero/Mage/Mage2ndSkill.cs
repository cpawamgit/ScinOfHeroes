using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage2ndSkill : Skills
{
    public Targetter targetter;
    public GameObject meteor;
    public float offset;


	

	protected override void Update ()
    {
        base.Update();

        if (skill == skill.dispo && targetter.target != null)
        {
            ActiveSkill();
        }
	}

    private void ActiveSkill()
    {
        Vector3 meteorSpawnPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        GameObject _meteor = PoolManager.Instance.poolDictionnary[meteor.name].GetFromPool(meteorSpawnPosition);
        Debug.Log("_meteor.GetComponent<Meteor>() = "+ _meteor.GetComponent<Meteor>());
        Debug.Log("targetter.target.position = " + targetter.target.position);


        _meteor.GetComponent<Meteor>().SetDestination(targetter.target.position, hero.GetAlignement());
        skill = skill.inCD;
    }




}
