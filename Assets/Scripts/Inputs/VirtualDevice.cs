using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class send inputs procedurally generated from a bot script
/// </summary>
public class VirtualDevice : PhysicalDevice
{
    Vector2 moveVector;

    public float changeDelay = 1f;
    private float time = 0;

    public override void Start()
    {
        device = GetComponent<DeviceEntity>();
        playerInput = null;
        time = Time.realtimeSinceStartup;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time > changeDelay)
        {
            time = 0;
            if(Random.value < 0.2f)
            {
                moveVector.x = Random.value > 0.5f ? -1 : 1;
                moveVector.y = 0;
            }
            else if (Random.value > 0.8f)
            {
                moveVector.x = 0;
                moveVector.y = Random.value > 0.5f ? -1 : 1;
            }
            else
            {
                moveVector.x = 0;
                moveVector.y = 0;
            }
        }
    }

    public override Vector2 GetMoveVector()
    {
        return moveVector;
    }
}
