using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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

#region NetworkMessages

public struct CreateMapMessage : NetworkMessage
{
    public byte mapSize;
    public byte boxPercent;
};
public struct ClearMapMessage : NetworkMessage {};




#endregion

public class LevelBuilder : NetworkBehaviour
{
    #region Properties

    /// <summary>
    /// Grid containing the Map data
    /// </summary>
    public static GenericGrid<Tile> grid;

    /// <summary>
    /// List containing the Empty Tiles of the map
    /// </summary>
    List<Tile> potentialBoxTile = new List<Tile>();

    /// <summary>
    /// List containing the Walls of the map
    /// </summary>
    List<Tile> wallTiles = new List<Tile>();

    /// <summary>
    /// List containing the Boxes of the map , use to put bonus on it
    /// </summary>
    List<ItemBox> actualBoxes = new List<ItemBox>();

    /// <summary>
    /// Responsible of assigning bonus on boxes
    /// </summary>
    BonusDispenser bonusDispenser;

    /// <summary>
    /// Dictionary containing the Meshes Shapes with their correponding rotations
    /// </summary>
    Dictionary<byte, MeshShape> wallShapes = new Dictionary<byte, MeshShape>();

    /// <summary>
    /// The total number of players to put on the map
    /// </summary>
    [Header("Global variables")]
    public SO_Int totalPlayersCount;

    /// <summary>
    /// Spacing between alternated walls
    /// </summary>
    [Range(2, 5)]
    public byte spacing;

    public Material blockMaterial;

    /// <summary>
    /// The size of the Map
    /// </summary>
    [Header("Map Size")]
    [Range(3, 10)] public byte mapSize;

    public SO_LevelBonusSettings bonusSettings;

    #region Meshes
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
    /// number of boxes in the map in pourcent
    /// </summary>
    [Range(0, 100)]
    public byte boxPercent;

    [HideInInspector]
    public Vector3[] playerStartPositions;

    static List<Tile> freeTiles = new List<Tile>();
    #endregion

    #region Init

    private void Start()
    {
        NetworkClient.RegisterHandler<CreateMapMessage>(CreateMapCallBack);
        NetworkClient.RegisterHandler<ClearMapMessage>(ClearMapCallBack);

        bonusDispenser = new BonusDispenser();

        CreateMeshDictionary();
    }

    private void Init()
    {
        //ClearMap();
        

        int mapDimension = GetMapDimension(mapSize);
        grid = new GenericGrid<Tile>(mapDimension, mapDimension, 1, Vector3.zero, TileConstructor);

        UpdateTilesPositions();

        CalculatePlayerStartPositions(totalPlayersCount.value);
        CalculateWallsAndBoxsPositions(spacing);
        CreateWalls();

        if (isServer)
        {
            CreateBoxes();
            AssignBonuses();
        }

        AssignPlayersPositions(true);
        ResetPlayers();
    }

    private void ResetPlayers()
    {
        int count = PlayerEntity.instancesList.Count;
       
        for (int i = 0; i < count; i++)
        {
            PlayerEntity.instancesList[i].gameObject.SendMessage("ResetVariables");
        }
    }

    private void UpdateTilesPositions()
    {
        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid.GetGridObject(x, z).worldPosition = grid.GetGridObjectWorldCenter(x, z);
            }
        }
    }

    private static void ClearGrid()
    {
        if (grid != null)
        {
            grid.ClearGridDebug();
            grid = null;
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

    private static void RemoveAllItems()
    {
        if (grid != null)
        {
            int width = grid.GetWidth();
            int height = grid.GetHeight();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Tile tile = grid.GetGridObject(x, z);
                    if (tile.item)
                    {
                        tile.item.Disable();
                    }
                }
            }
        }
    }

    public static Tile[] GetFreeTiles()
    {
        freeTiles.Clear();

        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile tile = grid.GetGridObject(i, j);

                if (tile.type == ElementType.Empty)
                {
                    freeTiles.Add(tile);
                }
            }
        }

        return freeTiles.ToArray();
    }
    #endregion

    private void AssignPlayersPositions(bool activatePlayers = false)
    {
        int count = Mathf.Min(PlayerEntity.instancesList.Count, playerStartPositions.Length);
        float size = grid.GetCellsize();
        Vector3 offset = new Vector3(grid.GetCellsize() / 2f, 0, grid.GetCellsize() / 2f);

        for (int i = 0; i < count; i++)
        {
            // send the position to the PlayerMovement script assigned to this player entity
            PlayerEntity.instancesList[i].SetSpawnPosition(playerStartPositions[i] * size + offset);
            if (activatePlayers)
            {
                PlayerEntity.instancesList[i].gameObject.SetActive(true);
            }
        }
    }

    private void CreateBoxes()
    {
        if (!isServer) return;

        int boxCount = Mathf.RoundToInt(potentialBoxTile.Count * (1 - (boxPercent/100f)));

        // Randomize boxes positions
        for (int i = 0; i < boxCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, potentialBoxTile.Count);
            potentialBoxTile.RemoveAt(randomIndex);
        }

        actualBoxes.Clear();
        for (int i = 0; i < potentialBoxTile.Count; i++)
        {
            //Calculate Box Position
            Vector3 boxTilePosition = grid.GetGridObjectWorldCenter(potentialBoxTile[i].x, potentialBoxTile[i].y);

            ItemBox box = PoolingSystem.instance.GetPoolObject(ItemsType.BOX, boxTilePosition) as ItemBox;

            actualBoxes.Add(box);
        }

    }

    void AssignBonuses() { 
    
        if(bonusDispenser != null && bonusSettings != null){

            bonusDispenser.bonusSettings = bonusSettings;
            bonusDispenser.AssignBonus(ref actualBoxes, totalPlayersCount.value);
        }

    }

    public Tile TileConstructor(GenericGrid<Tile> grid, int x, int y)
    {
        return new Tile(grid, x, y);
    }
    public int GetMapDimension(int mapSize)
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
    void CalculateWallsAndBoxsPositions(int spacing = 2)
    {
        int width = grid.GetWidth();
        int height = grid.GetHeight();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool isWall = (i % spacing == 0 && j % spacing == 0);
                isWall = isWall || (i == 0 || j == 0 || i == width - 1 || j == height - 1);

                if (isWall)
                    SetTileAsWall(i, j);
                else
                    if (!IsNearPlayerStartPosition(i, j))
                    potentialBoxTile.Add(grid.GetGridObject(i, j)); // Store potential boxPositions who are not near the playerPosition
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
        for (int i = 0; i < totalPlayersCount.value; i++)
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

        for (int i = 0; i < playerStartPositions.Length; i++) // Décaler d'une case si ca arrive sur un mur
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
    private void CreateMapCallBack(CreateMapMessage msg)
    {
        //Debug.Log("Receive Map CallBack");

        mapSize = msg.mapSize;
        boxPercent = msg.boxPercent;

        Init();

    }
    private void ClearMapCallBack(ClearMapMessage msg)
    {
        ClearMap();
    }

    private void ClearMap()
    {
        RemoveAllItems();
        potentialBoxTile.Clear();
        wallTiles.Clear();
        actualBoxes.Clear();
        ClearGrid();
    }

    private void OnGUI()
    {
        if (isServer)
        {
            if (GUI.Button(new Rect(300, 10, 200, 25), "Create Map"))
            {

                //NetworkServer.SendToAll(new ClearMapMessage { });

                //Debug.Log("Send CreateMap Message");


                CreateMap();
            }

            if (GUI.Button(new Rect(600, 10, 200, 25), "Clear Map"))
            {
                NetworkServer.SendToAll(new ClearMapMessage { });
            }

            if (GUI.Button(new Rect(500, 35, 100, 25), "Slow"))
            {
                Time.timeScale = 0.1f;
            }

            if (GUI.Button(new Rect(600, 35, 100, 25), "Normal"))
            {
                Time.timeScale = 1f;
            }
        }
    }


    bool buildingMap = false;
    public void CreateMap()
    {
        if (buildingMap) return;

        StartCoroutine(InitMap());
        
    }

    IEnumerator InitMap()
    {
        buildingMap = true;

        NetworkServer.SendToAll(new ClearMapMessage { });
        yield return new WaitForSecondsRealtime(0.1f);

        NetworkServer.SendToAll(new CreateMapMessage { mapSize = this.mapSize, boxPercent = this.boxPercent });
        yield return new WaitForSecondsRealtime(0.1f);

        NetworkServer.SendToAll(new GameStartedMessage { });
        yield return new WaitForSecondsRealtime(0.1f);

        buildingMap = false;
    }
    

}
