using System;
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
    List<Tile> potentialBoxPosition = new List<Tile>();

    /// <summary>
    /// List containing the Walls of the map
    /// </summary>
    List<Tile> wallTiles = new List<Tile>();

    /// <summary>
    /// Dictionary containing the Meshes Shapes with their correponding rotations
    /// </summary>
    Dictionary<byte, MeshShape> wallShapes = new Dictionary<byte, MeshShape>();

    /// <summary>
    /// The size of the Map
    /// </summary>
    [Header("Map Size")]
    [Range(3, 10)] public int mapSize;

    #region Meshs
    [Header("Wall Meshes")]
    public Mesh block_I;
    public Mesh block_T;
    public Mesh block_X;
    public Mesh block_L;
    public Mesh block_U;
    public Mesh block_O;
    #endregion

    public GameObject boxPrefab;

    /// <summary>
    /// number of boxs in the map in pourcent
    /// </summary>
    [Range(0, 100)]
    public int boxPrcent;

    public Vector3[] playerStartPositions;

    public Material blockMaterial;
    #endregion

    #region Init
    private void Init()
    {
        int mapDimension = GetMapDimension(mapSize);
        grid = new GenericGrid<Tile>(mapDimension, mapDimension, 1, Vector3.zero, TileConstructor);

        CreateMeshDictionary();

        CalculatePlayerStartPositions(DeviceInputs.instances.Count);

        CalculateWallsAndBoxsPositions();

        CreateWalls();

        CreateBoxs();
    } 
    #endregion

    private void CreateBoxs()
    {
        int boxCount = Mathf.RoundToInt(potentialBoxPosition.Count * (1-boxPrcent/100f));

        // Randomize boxs positions
        for (int i = 0; i < boxCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, potentialBoxPosition.Count);
            potentialBoxPosition.RemoveAt(randomIndex);
        }

        // Add offset to center the box on the middle of the tile 
        Vector3 offset = new Vector3(grid.GetCellsize() / 2f, 0, grid.GetCellsize() / 2f);

        // Instantiate the boxes
        for (int i = 0; i < potentialBoxPosition.Count; i++)
        {
            Vector3 boxTilePosition = grid.GetGridObjectWorldPosition(potentialBoxPosition[i].x, potentialBoxPosition[i].y);
            InstantiateBoxs(boxTilePosition + offset);
        }
    }
    private void InstantiateBoxs(Vector3 position)
    {
        Instantiate(boxPrefab, position, Quaternion.identity);
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
    void CalculateWallsAndBoxsPositions()
    {
        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool isWall = (i % 2 == 0 && j % 2 == 0);
                isWall = isWall || (i == 0 || j == 0 || i == width - 1 || j == height - 1);

                if (isWall)
                    SetTileAsWall(i, j);
                else
                    if (!IsNearPlayerStartPosition(i, j))
                    potentialBoxPosition.Add(grid.GetGridObject(i, j)); // Store potential boxPositions who are not near the playerPosition
            }
        }
    }
    private void SetTileAsWall(int i, int j)
    {
        Tile wallTile = grid.GetGridObject(i, j);
        wallTile.SetType(ElementType.Wall);
        wallTiles.Add(wallTile);
    }
    private void CreateWalls()
    {
        CombineInstance[] combine = new CombineInstance[wallTiles.Count];
        int meshCount = 0;

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                Tile tile = grid.GetGridObject(i, j);

                if (tile.type == ElementType.Wall)
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
    private bool IsNearPlayerStartPosition(int x, int z) 
    {
        for (int i = 0; i < DeviceInputs.instances.Count; i++)
        {
            if (Mathf.Abs(x - playerStartPositions[i].x) < 2 && Mathf.Abs(z - playerStartPositions[i].z) < 2)
            {
                return true;
            }
        }

        return false;
    }
    private void CalculatePlayerStartPositions(int count = 1)
    {

         playerStartPositions = new Vector3[10];
        int width = grid.GetWidth() - 1;
        int height = grid.GetHeight() - 1;

        // player 1
        playerStartPositions[0].x = 1;
        playerStartPositions[0].z = height - 1;

        // player 2
        playerStartPositions[1].x = width - 1;
        playerStartPositions[1].z = 1;

        // player 3
        playerStartPositions[2].x = width - 1;
        playerStartPositions[2].z = height - 1;

        // player 4
        playerStartPositions[3].x = 1;
        playerStartPositions[3].z = 1;

        switch (count)
        {
            case 5:
                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 2);
                playerStartPositions[4].z = Mathf.Round(height / 2);
                break;

            case 6:
                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 4);
                playerStartPositions[4].z = Mathf.Round(height / 2);

                // player 6
                playerStartPositions[5].x = width - Mathf.Round(width / 4);
                playerStartPositions[5].z = Mathf.Round(height / 2);
                break;

            case 7:
                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 2);
                playerStartPositions[4].z = Mathf.Round(height / 2);

                // player 6
                playerStartPositions[5].x = Mathf.Round(width / 5);
                playerStartPositions[5].z = Mathf.Round(height / 2);

                // player 7
                playerStartPositions[6].x = width - Mathf.Round(width / 5);
                playerStartPositions[6].z = Mathf.Round(height / 2);
                break;

            case 8:
                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 4);
                playerStartPositions[4].z = Mathf.Round(height / 2);

                // player 6
                playerStartPositions[5].x = width - Mathf.Round(width / 4);
                playerStartPositions[5].z = Mathf.Round(height / 2);

                // player 7
                playerStartPositions[6].x = Mathf.Round(width / 2);
                playerStartPositions[6].z = 1;

                // player 8
                playerStartPositions[7].x = Mathf.Round(width / 2);
                playerStartPositions[7].z = height - 1;
                break;

            case 9:
                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 4);
                playerStartPositions[4].z = Mathf.Round(height / 2);

                // player 6
                playerStartPositions[5].x = width - Mathf.Round(width / 4);
                playerStartPositions[5].z = Mathf.Round(height / 2);

                // player 7
                playerStartPositions[6].x = Mathf.Round(width / 2);
                playerStartPositions[6].z = 1;

                // player 8
                playerStartPositions[7].x = Mathf.Round(width / 2);
                playerStartPositions[7].z = height - 1;


                // player 9
                playerStartPositions[8].x = Mathf.Round(width / 2);
                playerStartPositions[8].z = Mathf.Round(height / 2);

                break;

            case 10:

                // player 5
                playerStartPositions[4].x = Mathf.Round(width / 4);
                playerStartPositions[4].z = Mathf.Round(height / 3);

                // player 6
                playerStartPositions[5].x = width - Mathf.Round(width / 4);
                playerStartPositions[5].z = Mathf.Round(height / 3);

                // player 7
                playerStartPositions[6].x = Mathf.Round(width / 2);
                playerStartPositions[6].z = 1;

                // player 8
                playerStartPositions[7].x = Mathf.Round(width / 2);
                playerStartPositions[7].z = height - 1;

                // player 9
                playerStartPositions[8].x = Mathf.Round(width / 4);
                playerStartPositions[8].z = height - Mathf.Round(height / 3);

                // player 10
                playerStartPositions[9].x = width - Mathf.Round(width / 4);
                playerStartPositions[9].z = height - Mathf.Round(height / 3);
                break;

        }

        for (int i = 0; i < playerStartPositions.Length; i++) // D�caler d'une case si ca arrive sur un mur
        {
            if (playerStartPositions[i].x % 2 == 0)
            {
                if (playerStartPositions[i].x < width / 2)
                {
                    playerStartPositions[i].x += 1;// Mathf.CeilToInt((float)xc / 10);

                }
                else if (playerStartPositions[i].x > width / 2)
                {
                    playerStartPositions[i].x -= 1;// Mathf.CeilToInt((float)xc / 10);
                }
                else
                {
                    playerStartPositions[i].z++;
                }
            }
        }

    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(300, 10, 200, 25), "Create Map"))
        {
            Init();
        }
    }
}
