using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoolableObject : NetworkBehaviour
{
    public static Vector3 hiddenPosition = new Vector3(0, -10, 0);

    NetworkTransform networkTransform;

    Animation animComponent;
    bool animState;

    MonoBehaviour[] components;
    bool[] states;

    GameObject[] children;
    bool[] childrenStates;

    [SyncVar] public bool activeState;

    bool componentsScanned;
    public string dbgName = "";

    public bool isActive
    {
        set
        {
            if (value)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        get
        {
            return activeState;
        }
    }

    public virtual void Awake()
    {
        dbgName = name;
        networkTransform = GetComponent<NetworkTransform>();
        animComponent = GetComponent<Animation>();
        activeState = true;
        ScanComponents();
        

        // Spawn only from server :
        // if this is the server/host
        // since the object is not yet instanced on the network it isServer property is not set,
        // NetworkServer.active is true and isClientOnly is false : spawn is done
        // 
        // if this is a client :
        // NetworkServer.active is false and isClientOnly is true or no set : spawn is not done

        if (NetworkServer.active && !isClientOnly)
        {
            NetworkServer.Spawn(gameObject);
        }

        // Disable AFTER NetworkServer.Spawn, to be sure ot disable the NetworkTransform component
        Disable();
    }

    private void ScanComponents()
    {

        components = this.gameObject.GetComponents<MonoBehaviour>();
        states = new bool[components.Length];

        children = new GameObject[this.transform.childCount];
        for(int i = 0; i < this.transform.childCount; i++)
        {
            children[i] = this.transform.GetChild(i).gameObject;
        }
        childrenStates = new bool[children.Length];

        componentsScanned = true;
        StoreState();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isActive)
        {
            Enable();
        }
        else
        {
            Disable();
        }

    }

    public void StoreState()
    {
        if (!componentsScanned) ScanComponents();

        for (int i = 0; i < components.Length; i++)
        {
            states[i] = components[i].enabled;
        }

        for (int i = 0; i < children.Length; i++)
        {
            childrenStates[i] = children[i].activeInHierarchy;
        }

        if (animComponent != null)
        {
            animState = animComponent.enabled;
        }
    }

    public virtual void Enable()
    {
        if (!activeState)
        {
            name = dbgName + netId +" Enabled";
            if (!componentsScanned) ScanComponents();

            for (int i = 0; i < components.Length; i++)
            {
                components[i].enabled = states[i];
            }

            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetActive(childrenStates[i]);
            }

            if (animComponent != null)
            {
                animComponent.enabled = animState;
            }

            activeState = true;
        }
    }

    public virtual void Disable()
    {
        if (activeState)
        {

            name = dbgName+" Disabled";
            StoreState();

            activeState = false;

            for (int i = 0; i < components.Length; i++)
            {
                components[i].enabled = false;
            }

            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetActive(false);
            }

            if (animComponent != null)
            {
                animComponent.enabled = false;
            }

          

            if (isServer)
            {
                Teleport(hiddenPosition);
            }
        }
        
    }

    public virtual void Teleport(Vector3 position)
    {
        if (isServer)
        {
            networkTransform.ServerTeleport(position);
        }
        else
        {
            //bool isEnabled = networkTransform.enabled;
            //networkTransform.enabled = false;
            transform.position = position;
            //networkTransform.enabled = isEnabled;
        }
    }

}
