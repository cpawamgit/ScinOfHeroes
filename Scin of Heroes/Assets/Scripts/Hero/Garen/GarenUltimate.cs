using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenUltimate : Skills {

    

    public int ultimateDamage = 1000;

    public UltimateTargetter ultimateTargetter;

    //private void UpdateTargets()
    //{
       
    //}

    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.R) && skill == skill.dispo)
        {
            Debug.Log("R been pressed !");
            foreach (GameObject enemy in ultimateTargetter.enemies)
            {
                IDamageable toDamage = enemy.GetComponent<IDamageable>();
                toDamage.TakeDamage(ultimateDamage);
            }
            skill = skill.inCD;
        }
    }

    
}
