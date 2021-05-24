using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct MeshShape
{
    public Mesh mesh;
    public float rotation;
}

[Flags]
public enum Direction : byte
{
    None = 0,
    North = 1,
    West = 1 << 1,
    East = 1 << 2,
    South = 1 << 3,
}

public class LevelBuilder : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Grid containing the Map data
    /// </summary>
    GenericGrid<Tile> grid;

    /// <summary>
    /// List containing the Empty Tiles of the map
    /// </summary>
    List<Tile> emptyTiles = new List<Tile>();

    /// <summary>
    /// List containing the Walls of the map
    /// </summary>
    List<Tile> wallTiles = new List<Tile>();

    /// <summary>
    /// Dictionary containing the Meshes Shapes with their correponding rotations
    /// </summary>
    Dictionary<byte, MeshShape> wallShapes = new Dictionary<byte, MeshShape>();

    public Material blockMaterial;

    public Vector3[] startPositions = new Vector3[10];

    [Header("Wall Meshes")]
    public Mesh block_I;
    public Mesh block_T;
    public Mesh block_X;
    public Mesh block_L;
    public Mesh block_U;
    public Mesh block_O;

    /// <summary>
    /// The size of the Map
    /// </summary>
    [Header("Map Size")]
    [Range(3, 10)] public int mapSize;

    #endregion

    private void Start()
    {
        int mapDimension = GetMapDimension(mapSize);
        grid = new GenericGrid<Tile>(mapDimension, mapDimension, 1, Vector3.zero, TileConstructor);

        CreateMeshDictionary();
        CreateLevelData();
        CombineMesh();
    }

    public Tile TileConstructor(GenericGrid<Tile> grid, int x, int y)
    {
        return new Tile(grid, x, y);
    }


    private int GetMapDimension(int mapSize)
    {
        return mapSize * 2 + 1;
    }
    private void CreateMeshDictionary()
    {
        // O BLOCKS
        wallShapes.Add(0, new MeshShape { mesh = block_O, rotation = 0 }); // ALL EMPTY

        // L BLOCKS (CORNER)
        wallShapes.Add(3, new MeshShape { mesh = block_L, rotation = 180 }); // TOP AND LEFT NOT EMPTY
        wallShapes.Add(5, new MeshShape { mesh = block_L, rotation = 270 });  // TOP AND RIGHT NOT EMPTY
        wallShapes.Add(10, new MeshShape { mesh = block_L, rotation = 90 }); // BOTTOM AND LEFT NOT EMPTY
        wallShapes.Add(12, new MeshShape { mesh = block_L, rotation = 0 }); // BOTTOM AND RIGHT NOT EMPTY

        // I BLOCKS (WALLS)
        wallShapes.Add(6, new MeshShape { mesh = block_I, rotation = 90 }); // LEFT AND RIGHT NOT EMPTY
        wallShapes.Add(9, new MeshShape { mesh = block_I, rotation = 0 }); // TOP AND BOTTOM NOT EMPTY

    }


    // INIT les startPositions en fonction du nompbre de joeurs InitPlayerStartPositions
    // FAIRE LES MURS 
    // Réperer les cases vides
    // CREER LES BOITES  dans les cases vides sauf sur les cases proche de la startposition d'un joueur (IsNearPlayerStartPosition)
    // spécifier un pourcentage de boite qui sont placée aléatoirement dans les cases vides



    void CreateLevelData()
    {
        int xc = grid.GetWidth();
        int zc = grid.GetHeight();

        for (int i = 0; i < xc; i++)
        {
            for (int j = 0; j < zc; j++)
            {
                bool isWall = (i % 2 == 0 && j % 2 == 0);
                isWall = isWall || (i == 0 || j == 0 || i == xc - 1 || j == zc - 1);

                if (isWall)
                {

                    Tile wallTile = grid.GetGridObject(i, j);
                    wallTile.SetType(ElementType.Wall);
                    wallTiles.Add(wallTile);
                }


                else
                {
                    
                    if (!IsNearPlayerStartPosition(i, j))
                    {
                        
                    }

                    Tile emptyTile = grid.GetGridObject(i, j);
                    emptyTile.SetType(ElementType.Empty);
                    emptyTiles.Add(emptyTile);

                }

            }
        }
    }
    private void CombineMesh()
    {
        CombineInstance[] combine = new CombineInstance[wallTiles.Count];
        int meshCount = 0;

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                Tile tile = grid.GetGridObject(i, j);

                if (tile.type != ElementType.Empty)
                {
                    byte wallShape = GetWallShapeIndex(grid.GetGridObject(i, j));

                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.localPosition = grid.GetGridObjectWorldPosition(i, j);

                    Mesh mesh;
                    MeshShape meshShape;
                    wallShapes.TryGetValue(wallShape, out meshShape);
                    mesh = meshShape.mesh;

                    if (mesh != null)
                    {
                        transform.rotation = Quaternion.Euler(0, meshShape.rotation, 0);

                        Mesh m_NewMesh = new Mesh();
                        m_NewMesh.name = (mesh.name + "_patches");
                        m_NewMesh.vertices = mesh.vertices;
                        m_NewMesh.bounds = mesh.bounds;
                        m_NewMesh.triangles = mesh.triangles;
                        m_NewMesh.normals = mesh.normals;
                        Vector2[] uvs = new Vector2[mesh.uv.Length];
                        Vector2 uvOffset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                        for (int u = 0; u < mesh.uv.Length; u++)
                        {
                            Vector2 uv = mesh.uv[u];
                            uvs[u] = uv + uvOffset;
                        }
                        m_NewMesh.uv = uvs;
                        combine[meshCount].mesh = m_NewMesh;

                        m_NewMesh = null;
                        combine[meshCount].transform = transform.localToWorldMatrix;
                        meshCount++;
                    }
                }
            }
        }
        transform.localPosition = Vector3.zero + new Vector3(grid.GetCellsize() / 2f, grid.GetCellsize() / 2f, grid.GetCellsize() / 2f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (combine.Length > 0)
        {
            MeshFilter mf = gameObject.GetComponent<MeshFilter>();
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            mr.sharedMaterial = blockMaterial;
            mf.mesh = new Mesh();
            mf.mesh.CombineMeshes(combine, true, true);
            gameObject.SetActive(true);
        }
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    public byte GetWallShapeIndex(Tile tile)
    {
        Direction direction = Direction.None;

        // WEST
        if (tile.x - 1 >= 0)
            if (grid.GetGridObject(tile.x - 1, tile.y).type == ElementType.Wall)
                direction |= Direction.West;

        // EAST
        if (tile.x + 1 < grid.GetWidth())
            if (grid.GetGridObject(tile.x + 1, tile.y).type == ElementType.Wall)
                direction |= Direction.East;

        //SOUTH
        if (tile.y - 1 >= 0)
            if (grid.GetGridObject(tile.x, tile.y - 1).type == ElementType.Wall)
                direction |= Direction.South;

        //NORTH
        if (tile.y + 1 < grid.GetHeight())
            if (grid.GetGridObject(tile.x, tile.y + 1).type == ElementType.Wall)
                direction |= Direction.North;

        return (byte)direction;
    }
    private bool IsNearPlayerStartPosition(int x, int z) // EXCLUT CERTAINES CASES POUR EN PLACER DES BOITES DESSUS
    {
        for (int i = 0; i < DeviceInputs.instances.Count; i++)
        {
            if (Mathf.Abs(x - startPositions[i].x) < 2 && Mathf.Abs(z - startPositions[i].z) < 2)
            {
                return true;
            }
        }

        return false;
    }
    private void InitPlayerStartPositions(int count = 1)
    {
        int xc = grid.GetWidth() - 1;
        int zc = grid.GetHeight() - 1;

        // player 1
        startPositions[0].x = 1;
        startPositions[0].z = zc - 1;

        // player 2
        startPositions[1].x = xc - 1;
        startPositions[1].z = 1;

        // player 3
        startPositions[2].x = xc - 1;
        startPositions[2].z = zc - 1;

        // player 4
        startPositions[3].x = 1;
        startPositions[3].z = 1;

        switch (count)
        {
            case 5:
                // player 5
                startPositions[4].x = Mathf.Round(xc / 2);
                startPositions[4].z = Mathf.Round(zc / 2);
                break;

            case 6:
                // player 5
                startPositions[4].x = Mathf.Round(xc / 4);
                startPositions[4].z = Mathf.Round(zc / 2);

                // player 6
                startPositions[5].x = xc - Mathf.Round(xc / 4);
                startPositions[5].z = Mathf.Round(zc / 2);
                break;

            case 7:
                // player 5
                startPositions[4].x = Mathf.Round(xc / 2);
                startPositions[4].z = Mathf.Round(zc / 2);

                // player 6
                startPositions[5].x = Mathf.Round(xc / 5);
                startPositions[5].z = Mathf.Round(zc / 2);

                // player 7
                startPositions[6].x = xc - Mathf.Round(xc / 5);
                startPositions[6].z = Mathf.Round(zc / 2);
                break;

            case 8:
                // player 5
                startPositions[4].x = Mathf.Round(xc / 4);
                startPositions[4].z = Mathf.Round(zc / 2);

                // player 6
                startPositions[5].x = xc - Mathf.Round(xc / 4);
                startPositions[5].z = Mathf.Round(zc / 2);

                // player 7
                startPositions[6].x = Mathf.Round(xc / 2);
                startPositions[6].z = 1;

                // player 8
                startPositions[7].x = Mathf.Round(xc / 2);
                startPositions[7].z = zc - 1;
                break;

            case 9:
                // player 5
                startPositions[4].x = Mathf.Round(xc / 4);
                startPositions[4].z = Mathf.Round(zc / 2);

                // player 6
                startPositions[5].x = xc - Mathf.Round(xc / 4);
                startPositions[5].z = Mathf.Round(zc / 2);

                // player 7
                startPositions[6].x = Mathf.Round(xc / 2);
                startPositions[6].z = 1;

                // player 8
                startPositions[7].x = Mathf.Round(xc / 2);
                startPositions[7].z = zc - 1;


                // player 9
                startPositions[8].x = Mathf.Round(xc / 2);
                startPositions[8].z = Mathf.Round(zc / 2);

                break;

            case 10:

                // player 5
                startPositions[4].x = Mathf.Round(xc / 4);
                startPositions[4].z = Mathf.Round(zc / 3);

                // player 6
                startPositions[5].x = xc - Mathf.Round(xc / 4);
                startPositions[5].z = Mathf.Round(zc / 3);

                // player 7
                startPositions[6].x = Mathf.Round(xc / 2);
                startPositions[6].z = 1;

                // player 8
                startPositions[7].x = Mathf.Round(xc / 2);
                startPositions[7].z = zc - 1;

                // player 9
                startPositions[8].x = Mathf.Round(xc / 4);
                startPositions[8].z = zc - Mathf.Round(zc / 3);

                // player 10
                startPositions[9].x = xc - Mathf.Round(xc / 4);
                startPositions[9].z = zc - Mathf.Round(zc / 3);
                break;

        }

        for (int i = 0; i < startPositions.Length; i++) // Décaler d'une case si ca arrive sur un mur
        {
            if (startPositions[i].x % 2 == 0)
            {
                if (startPositions[i].x < xc / 2)
                {
                    startPositions[i].x += 1;// Mathf.CeilToInt((float)xc / 10);

                }
                else if (startPositions[i].x > xc / 2)
                {
                    startPositions[i].x -= 1;// Mathf.CeilToInt((float)xc / 10);
                }
                else
                {
                    startPositions[i].z++;
                }
            }
        }

    }

}
