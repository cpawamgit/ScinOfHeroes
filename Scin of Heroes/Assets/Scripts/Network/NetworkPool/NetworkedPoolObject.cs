using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/*
*	Author: Juan Camilo Charria
*	Website: http://www.justjuank.com
*	Description: This class represents an object being used by a NHNetworkedPool
*/

public class NetworkedPoolObject : NetworkBehaviour {

    [SyncVar]
    public bool isObjectActive = true;
    bool locallyActive = true;

    void Update()
    {
        //Client checks to active or inactive this object locally based on the server version state @isObjectActive
        if (NetworkServer.active && !isServer)
        {
            if (isObjectActive && !locallyActive)
            {
                SetObjectActive();
            }
            if (!isObjectActive && locallyActive)
            {
                SetObjectInactive();
            }
        }
    }
    
    public void SetObjectActive()
    {
        DoSetObjectActive();

        if (isServer)
        {
            isObjectActive = true;
            RpcSetObjectActive();
        }
    }
    [ClientRpc]
    void RpcSetObjectActive()
    {
        DoSetObjectActive();
    }
    private void DoSetObjectActive()
    {
        Component[] comps = GetComponents<Component>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] != this && comps[i].GetType() != typeof(NetworkIdentity))
            {
                if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    ((MonoBehaviour)comps[i]).enabled = true;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider)))
                    ((Collider)comps[i]).enabled = true;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider2D)))
                    ((Collider2D)comps[i]).enabled = true;

                if (comps[i].GetType().IsSubclassOf(typeof(Renderer)))
                    ((Renderer)comps[i]).enabled = true;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        locallyActive = true;
    }
    

    public void SetObjectInactive()
    {
        DoSetObjectInactive();

        if (isServer)
        {
            isObjectActive = false;
            if (transform.parent && transform.parent.GetComponent<NetworkedPool>())
            {
                transform.parent.GetComponent<NetworkedPool>().AddToAvailableObjects(gameObject);
            }
            RpcSetObjectInactive();
        }
    }
    [ClientRpc]
    void RpcSetObjectInactive()
    {
        DoSetObjectInactive();
    }

    private void DoSetObjectInactive()
    {
        Component[] comps = GetComponents<Component>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] != this && comps[i].GetType() != typeof(NetworkIdentity))
            {
                if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    ((MonoBehaviour)comps[i]).enabled = false;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider)))
                    ((Collider)comps[i]).enabled = false;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider2D)))
                    ((Collider2D)comps[i]).enabled = false;

                if (comps[i].GetType().IsSubclassOf(typeof(Renderer)))
                    ((Renderer)comps[i]).enabled = false;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        locallyActive = true;
    }    
}
