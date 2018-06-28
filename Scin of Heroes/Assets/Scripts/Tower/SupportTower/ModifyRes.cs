using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyRes : MonoBehaviour
{

    public float resModificator;

    public GameObject resEffect;
    private GameObject _resEffect;


    public void ChangeRes()
    {
        GetComponent<IDamageable>().ChangeRes(resModificator, "add");

        _resEffect = Instantiate(resEffect);                            ///// PASSER PAR LA POOL
        _resEffect.transform.parent = gameObject.transform;
    }

    private void OnDisable()
    {
        GetComponent<IDamageable>().ChangeRes(resModificator, "remove");        ///// PASSER PAR LA POOL
        Destroy(_resEffect);
    }


}
