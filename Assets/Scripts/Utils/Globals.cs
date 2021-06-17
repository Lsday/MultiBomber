using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static Vector3Int[] Vector3_directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
    public static Vector2Int[] Vector2_directions = { Vector2Int.up, Vector2Int.left, Vector2Int.right, Vector2Int.down };

    public static int GetDirectionIndex(int x, int z)
    {
        for(int i = 0; i < Vector3_directions.Length; i++)
        {
            if(Vector3_directions[i].x == x && Vector3_directions[i].z == z)
            {
                return i;
            }
        }

        return 0;
    }
}
