
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;
public class ItemKickable : ItemBase, IKickable
{
    public bool kicked = false;
    PlayerEntity kicker;

    Vector3 direction;
    public float speed = 6;

    int kickPower = 0;
    GenericGrid<Tile> grid;

    SphereCollider sphereCollider;
    public override void Awake()
    {
        base.Awake();

        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.enabled = false;

    }

    public void Kick(PlayerEntity playerEntity, int power, int h, int v)
    {
        if (isServer && (h == 0 || v == 0)) // one axis must be 0
        {
            int index = PlayerEntity.GetEntityIndex(playerEntity);
            RpcKick((byte)index, (byte)power, (byte)(h + 1), (byte)(v + 1));
        }
    }


    [Command]
    public void CmdKick(byte entityIndex, byte power, byte h, byte v)
    {
        RpcKick(entityIndex, power, h, v);
    }

    [ClientRpc]
    public void RpcKick(byte entityIndex, byte power, byte h, byte v)
    {
        if (kicked) return;

        if (grid == null)
        {
            grid = LevelBuilder.grid;
        }

        direction.x = ((int)h)-1;
        direction.z = ((int)v)-1;

        Tile obj = grid.GetGridObject(myTransform.position + direction);

        if (obj.type >= ElementType.Block) {
            return;
        }

        kicker = PlayerEntity.GetEntity(entityIndex);
        kickPower = power;
        kicked = true;

        sphereCollider.enabled = true;

        StartCoroutine(KickUpdate());
    }

    IEnumerator KickUpdate()
    {
        while (kicked)
        {
            Vector3 position = myTransform.position;
            Tile tile = grid.GetGridObject(position);
            Vector3 destination = tile.worldPosition;

            if (tile.temperature > 0)
            {
                kicked = false;
                position = destination;
                StopKick();

                if (isServer)
                {
                    ((IDestroyable)this).RpcInitDestroy(0.0f, 0.2f);
                }
            }
            else
            {

                tile = grid.GetGridObject(position + direction);

                if (tile.type >= ElementType.Block)
                {
                    // stop on obstacle

                    if (Vector3.Distance(destination, position) < 0.1f)
                    {
                        kicked = false;
                        position = destination;

                        if (tile.type == this.type && kickPower > 1 && tile.item != null)
                        {
                            ((ItemKickable)tile.item).Kick(kicker, kickPower - 1, (int)direction.x, (int)direction.z);
                            StopKick();
                        }
                    }
                }
                else if (PlayerEntity.GetMajorPlayerOccupation(tile.x, tile.y) > 0.5f) // a player is more than 50% on the tile
                {
                    // stop on player

                    if (Vector3.Distance(destination, position) < 0.1f)
                    {
                        kicked = false;
                        position = destination;
                        StopKick();
                    }
                }
                else if (tile.type == ElementType.Item)
                {
                    // destroy bonus on the way
                    destination = grid.GetGridObjectWorldCenter(position + direction);

                    if (Vector3.Distance(destination, position) < 1f)
                    {
                        ((ItemBonus)tile.item).InitDestroy(0, 0.2f);
                    }
                }
            }

            RemoveFromTile();

            if (kicked){
                position += direction * speed * Time.deltaTime;
            }
            myTransform.position = position;

            PlaceOnTile(position);

            yield return new WaitForSeconds(Time.deltaTime);
        }
        sphereCollider.enabled = false;
    }



    public void StopKick()
    {
        StopAllCoroutines();
        sphereCollider.enabled = false;
    }

    public override void Disable()
    {
        
        kicked = false;
        kicker = null;
        GetComponent<Collider>().enabled = false;
        base.Disable();
    }

    
}
