using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKickable : ItemBase, IKickable
{
    public bool kicked = false;
    PlayerEntity kicker;

    Vector3 direction;
    public float speed = 6;

    int kickPower = 0;
    GenericGrid<Tile> grid;

    public void Kick(PlayerEntity playerEntity, int power, int h, int v)
    {
        if (kicked) return;

        if (grid == null)
        {
            grid = LevelBuilder.grid;
        }

        direction.x = h;
        direction.z = v;

        Tile obj = grid.GetGridObject(myTransform.position + direction);

        if (obj.type >= ElementType.Block) {
            return;
        }

        kicker = playerEntity;
        kickPower = power;
        kicked = true;

        StartCoroutine(KickUpdate());
    }

    IEnumerator KickUpdate()
    {
        while (kicked)
        {
            Vector3 position = myTransform.position;
            Tile obj = grid.GetGridObject(position + direction);
            
            if (obj.type >= ElementType.Block)
            {
                Vector3 destination = grid.GetGridObjectWorldCenter(position);

                if(Vector3.Distance(destination , position) < 0.1f)
                {
                    kicked = false;
                    position = destination;

                    if (obj.type == ElementType.Bomb && kickPower > 1)
                    {
                        ((ItemBomb)obj.item).Kick(kicker, kickPower - 1, (int)direction.x, (int)direction.z);
                    }
                }

            }else if (obj.type == ElementType.Item)
            {
                Vector3 destination = grid.GetGridObjectWorldCenter(position + direction);

                if (Vector3.Distance(destination, position) < 1f)
                {
                    ((ItemBonus)obj.item).InitDestroy(0, 0.2f);
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
    }



    public void StopKick()
    {
        StopAllCoroutines();
    }

    public override void Disable()
    {
        base.Disable();
        kicked = false;
        kicker = null;
    }

    
}
