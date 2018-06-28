using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard_Photon : MonoBehaviour
{

	void Update ()
    {
        transform.LookAt(Camera.main.transform);
    }
}
