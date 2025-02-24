using UnityEngine;
using Mirror;
using System;

public class PlayerMovement : NetworkBehaviour
{

    public Action<int,int> OnPlayerMoved;
    public Action OnPlayerStopMoved;

    public struct TileOccupation
    {
        public Tile tile;
        public float occupation;
    }

    #region Properties
    bool isRunning;
    PlayerEntity playerEntity;
    NetworkTransform networkTransform;

    private float matchSpeed;

    public float startSpeed = 3;
    [SyncVar] private float speed = 3;
    public float speedMax = 10;

    [Range(0f, 0.5f)]
    public float cornerSliding = 0f;

    public SO_Bool gameStarted;

    public int currentTileX;
    public int currentTileY;

    private Tile currentTile;

    public TileOccupation[] tiles = new TileOccupation[4];

    private Vector3 currentTileCenter;
    private Vector3 currentOffsetFromCenter;
    private Vector3 movementClampLow;
    private Vector3 movementClampHigh;
    private Vector3 lastPosition;

    public Filter currentFilter;

    GenericGrid<Tile> grid;

    public Action OnTileEntered;

    int horizontalInput;
    int verticalInput;

    Vector2Int currentDirection;
    Vector2Int previousInput;
    #endregion

    public PlayerAnimations playerAnimations;

    /// <summary>
    /// the collision layer index to detect kicked elements
    /// </summary>
    int movingObstaclesMask;

    public float Speed
    {
        get
        {
            return playerEntity.currentInputData.speed;
        }
    }


    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
        lastPosition = transform.position;
        transform.forward = Vector3.back;
        currentDirection = Vector2Int.down;

        networkTransform = GetComponent<NetworkTransform>();

        playerEntity.OnPlayerSpawnPosition += SetWorldPosition;
        playerEntity.OnPlayerDied += ResetVariables;
        
        movingObstaclesMask = LayerMask.GetMask("MovingObstacles");

        ResetVariables();
    }

    private void ResetVariables()
    {
        speed = startSpeed;
        currentTileX = 0;
        currentTileY = 0;
    }

    public void SetWorldPosition(Vector3 position)
    {
        grid = LevelBuilder.grid;

        transform.position = position;
        
        transform.forward = Vector3.back;
        currentDirection = Vector2Int.down;

        UpdateTileCoordinates();
        ComputeMovementLimits();
    }

    void ReadInputs()
    {

        if (playerEntity.hubIdentity.isLocalPlayer)
        {
            playerEntity.currentInputData.movement = playerEntity.controllerDevice.inputs.GetMoveVector();
            playerEntity.currentInputData.direction = currentDirection;
        }

        // set player speed for both host and client, to get the right running animation speed 
        playerEntity.currentInputData.speed = speed;

        if (currentFilter)
        {
            currentFilter.FilterData(ref playerEntity.currentInputData);
        }
    }

    bool prevRunning;
    void Update()
    {
        if (!gameStarted.value) return;

        if (playerEntity.isDead || playerEntity.IsLocked) return;


        if (playerEntity.hubIdentity.isLocalPlayer)
        {
            if (playerEntity.controllerDevice == null || !Application.isFocused) return;

            // 
            ReadInputs();

            float horizontal = playerEntity.currentInputData.movement.x;
            float vertical = playerEntity.currentInputData.movement.y;

            //bool prevRunning = isRunning;

            if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 0.01f)
            {
                isRunning = Move(horizontal, vertical);
            }
            else
            {
                isRunning = false;
            }

            int newH = Utils.RoundedSign(horizontal, 0.5f);
            int newV = Utils.RoundedSign(vertical, 0.5f);

            if (prevRunning != isRunning || newH != horizontalInput || newV != verticalInput)
            {
                CmdSyncRunning(isRunning, newH, newV);
            }
            horizontalInput = newH;
            verticalInput = newV;

            prevRunning = isRunning;
        }
        else
        {
            currentDirection.x = Utils.RoundedSign(transform.forward.x,0.5f);
            currentDirection.y = Utils.RoundedSign(transform.forward.z,0.5f);

            ReadInputs();

            Vector3 difference = transform.position - lastPosition;
            if (Mathf.Abs(difference.x) + Mathf.Abs(difference.z) > 0.01f)
            {
                UpdateTileCoordinates();
                lastPosition = transform.position;
            }
        }

        if (horizontalInput != 0 || verticalInput != 0)
        {
            OnPlayerMoved?.Invoke(horizontalInput, verticalInput);
        }
       
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + new Vector3(horizontalInput, 0, verticalInput),Color.yellow);

        // ANIM-TODO : � bouger
        playerAnimations.UpdateAnimations(isRunning);
    }

    [Command]
    void CmdSyncRunning(bool runningState, int hInput, int vInput)
    {
        RpcSyncRunning(runningState, hInput, vInput);
    }

    [ClientRpc]
    void RpcSyncRunning(bool runningState, int hInput, int vInput)
    {
        isRunning = runningState;
        horizontalInput = hInput;
        verticalInput = vInput;
    }

    private bool Move(float h, float v)
    {

        UpdateTileCoordinates();

        ComputeMovementLimits();
        
        UpdatePositionClamping();

        lastPosition = transform.position;
        Vector3 newPosition = lastPosition;
        float travelDistance = (Time.deltaTime * Mathf.Min(matchSpeed , playerEntity.currentInputData.speed));

        int moveX = Utils.RoundedSign(h);
        int moveZ = Utils.RoundedSign(v);

        Vector3 dir = new Vector3(moveX, 0, moveZ);

        newPosition += dir.normalized * travelDistance;

        newPosition = ComputeCorners(lastPosition, newPosition, dir, travelDistance);

        newPosition.x = Mathf.Clamp(newPosition.x, movementClampLow.x, movementClampHigh.x);
        newPosition.z = Mathf.Clamp(newPosition.z, movementClampLow.z, movementClampHigh.z);

        transform.position = newPosition;

        // orientation
        dir = (newPosition - lastPosition);
        int signX = Utils.RoundedSign(dir.x);
        int signZ = Utils.RoundedSign(dir.z);

        if (signX == 0 && signZ == 0)
        {

            if (moveX != previousInput.x || moveZ == 0 && moveX != currentDirection.x)
            {
                signX = moveX;
            }
            
            if (moveZ != previousInput.y || moveX == 0 && moveZ != currentDirection.y)
            {
                signZ = moveZ;
            }
        }
        else
        {
            if (signX != moveX)
            {
                signX = 0;
            }

            if (signZ != moveZ)
            {
                signZ = 0;
            }
        }

        if (signX != 0 || signZ != 0)
        {
            currentDirection.x = signX;
            currentDirection.y = signZ;

            transform.forward = new Vector3(signX, 0, signZ).normalized;
        }

        playerEntity.currentInputData.direction = currentDirection;

        previousInput.x = moveX;
        previousInput.y = moveZ;


        return signX != 0 || signZ != 0;
    }

    [ClientRpc]
    public void RpcModifySpeed(float amount)
    {
        speed = Mathf.Clamp(speed + amount, startSpeed, speedMax);
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].tile != null)
            {

                Gizmos.color = new Color(0f, 1f, 0f, tiles[i].occupation);
                Gizmos.DrawWireCube(tiles[i].tile.worldPosition, Vector3.one*0.95f);

                if (tiles[i].occupation > playerEntity.deathThreshold)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(tiles[i].tile.worldPosition, Vector3.one * 0.3f);

                }
            }

        }

        /*

        Gizmos.color = dbgCol;

        //Gizmos.color = Color.red;
        if (LevelBuilder.grid != null)
        {
            Gizmos.DrawWireCube(currentTileCenter, Vector3.one * 0.95f);
        }

        //Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(debugPivot, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + debugDir);

        //Gizmos.color = pivotWall ? Color.red : Color.green;
        Gizmos.DrawWireSphere(testWallPivot, 0.49f);

        //Gizmos.color = destWall ? Color.magenta : Color.yellow;
        Gizmos.DrawWireCube(testWallDest, Vector3.one * 0.4f);

        //Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(intersect, 0.3f);
        */
    }

    [ClientRpc]
    private void RpcUpdateTileCoordinates()
    {
        UpdateTileCoordinates();
    }

    private void UpdateTileCoordinates()
    {
        if (grid == null) return;

        int tilex, tiley;

        grid.GetXY(transform.position, out tilex, out tiley);

        if (currentTileX != tilex || currentTileY != tiley)
        {
           

            currentTile = grid.GetGridObject(transform.position);

            if (currentTile != null)
            {
                currentTileX = currentTile.x;
                currentTileY = currentTile.y;
                currentTileCenter = grid.GetGridObjectWorldCenter(currentTileX, currentTileY);
            }

            OnTileEntered?.Invoke();
        }

        currentOffsetFromCenter = transform.position - currentTileCenter;

        ComputeTilesOccupation();
    }

    private void ComputeTilesOccupation()
    {
        int signX = Utils.RoundedSign(currentOffsetFromCenter.x);
        int signY = Utils.RoundedSign(currentOffsetFromCenter.z);

        tiles[0].tile = currentTile;
        tiles[1].tile = signX == 0 ? null : grid.GetGridObject(currentTileX + signX, currentTileY);
        tiles[2].tile = signY == 0 ? null : grid.GetGridObject(currentTileX, currentTileY + signY);
        tiles[3].tile = signX == 0 || signY == 0 ? null : grid.GetGridObject(currentTileX + signX, currentTileY + signY);

        Vector3 playerOrigin = new Vector3(transform.position.x - 0.5f, 0, transform.position.z - 0.5f);
        for (int i = 0; i < 4; i++)
        {
            if (tiles[i].tile != null)
            {
                Vector3 origin = grid.GetGridObjectWorldPosition(tiles[i].tile.x, tiles[i].tile.y);

                float x_overlap = Mathf.Max(0, Mathf.Min(playerOrigin.x + 1, origin.x + 1) - Mathf.Max(playerOrigin.x, origin.x));
                float z_overlap = Mathf.Max(0, Mathf.Min(playerOrigin.z + 1, origin.z + 1) - Mathf.Max(playerOrigin.z, origin.z));

                tiles[i].occupation = x_overlap * z_overlap;
            }
            else
            {
                tiles[i].occupation = 0;
            }
        }
    }


    private void ComputeMovementLimits()
    {
        if (grid == null) return;

        Tile tileLeft = grid.GetGridObject(currentTileX - 1, currentTileY);
        Tile tileRight = grid.GetGridObject(currentTileX + 1, currentTileY);

        Tile tileUp = grid.GetGridObject(currentTileX, currentTileY + 1);
        Tile tileDown = grid.GetGridObject(currentTileX, currentTileY - 1);

        bool leftWall = tileLeft.type >= ElementType.Block;
        bool rightWall = tileRight.type >= ElementType.Block;

        bool upWall = tileUp.type >= ElementType.Block;
        bool downWall = tileDown.type >= ElementType.Block;

        float cellSize = grid.GetCellsize();
        
        Vector3 minOffset = new Vector3(leftWall ? 0 : -cellSize, 0, downWall ? 0 : -cellSize);
        Vector3 maxOffset = new Vector3(rightWall ? 0 : cellSize, 0, upWall ? 0 : cellSize);

        movementClampLow = currentTileCenter + minOffset;
        movementClampHigh = currentTileCenter + maxOffset;

        movementClampLow.x = Mathf.Min(movementClampLow.x, transform.position.x);
        movementClampLow.z = Mathf.Min(movementClampLow.z, transform.position.z);

        movementClampHigh.x = Mathf.Max(movementClampHigh.x, transform.position.x);
        movementClampHigh.z = Mathf.Max(movementClampHigh.z, transform.position.z);
    }


    public void UpdatePositionClamping()
    {

        // prevent the player to move faster than a kicked bomb on its way
        // TODO : maybe this is cleaner to move this part in the PlayerCollision class
        RaycastHit hit;
        matchSpeed = playerEntity.currentInputData.speed;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.1f, movingObstaclesMask))
        {

            ItemKickable obj = hit.collider.GetComponent<ItemKickable>();
            if (obj != null && hit.distance < 1f) matchSpeed = obj.speed * 0.9f;

            if (currentDirection.x < 0)
            {
                movementClampLow.x = hit.transform.position.x + 1f;
            }

            if (currentDirection.x > 0)
            {
                movementClampHigh.x = hit.transform.position.x - 1f;
            }

            if (currentDirection.y < 0)
            {
                movementClampLow.z = hit.transform.position.z + 1f;
            }

            if (currentDirection.y > 0)
            {
                movementClampHigh.z = hit.transform.position.z - 1f;
            }
        }
    }

    //TODO : remove debug
    Vector3 debugPivot;
    Vector3 debugDir;
    Vector3 testWallPivot;
    Vector3 testWallDest;
    Vector3 intersect;
    Color dbgCol;

    private Vector3 ComputeCorners(Vector3 startPosition, Vector3 position, Vector3 travelDirection, float travelDistance)
    {
        dbgCol = Color.black;//TODO : remove debug

        debugDir = travelDirection;//TODO : remove debug

        if (grid == null) return position;

        // offset between the center of the ciurrent tile and the player position
        Vector3 offset = position - currentTileCenter;

        // get position sign within the Tile
        // posSignX positive = the player is on the right side of the Tile
        // posSignZ positive = the player is on the upper side of the tile
        int posSignX = Utils.RoundedSign(offset.x, 0.01f);
        int posSignZ = Utils.RoundedSign(offset.z, 0.01f);

        // the player is perfectly aligned on an axis, no turn is possible in this case
        if (posSignX == 0 || posSignZ == 0)
        {
            dbgCol = Color.white;//TODO : remove debug
            return position;
        }
        // detect if there is a wall to turn around it 
        // true = a blocking wall is present
        // false = the tile is empty
        bool blockingPivotTile = grid.GetGridObject(currentTileX + posSignX, currentTileY + posSignZ).type >= ElementType.Block;

        // move freely if if no pivot tile is present
        bool checkAreaX = grid.GetGridObject(currentTileX + posSignX, currentTileY).type >= ElementType.Block;
        bool checkAreaZ = grid.GetGridObject(currentTileX, currentTileY + posSignZ).type >= ElementType.Block;
        if (!blockingPivotTile && !checkAreaX && !checkAreaZ)
        {
            dbgCol = Color.magenta;//TODO : remove debug
            return position;
        }

        // sign of the travel direction on each axis
        int dirSignX = Utils.RoundedSign(travelDirection.x);
        int dirSignZ = Utils.RoundedSign(travelDirection.z);

        // detect is the destination tile is blocking or not
        bool blockingDestTile = grid.GetGridObject(currentTileX + dirSignX, currentTileY + dirSignZ).type >= ElementType.Block;

        // detect blocking wall in each axis
        bool blockingX = grid.GetGridObject(currentTileX + dirSignX, currentTileY).type >= ElementType.Block;
        bool blockingZ = grid.GetGridObject(currentTileX, currentTileY + dirSignZ).type >= ElementType.Block;

        if (blockingDestTile)
        {
            int dx = dirSignX;
            int dz = dirSignZ;
            if (blockingX && dirSignX != 0 && dirSignZ != 0)
            {
                dx = 0;
            }

            if (blockingZ && dirSignX != 0 && dirSignZ != 0)
            {
                dz = 0;
            }

            blockingDestTile = grid.GetGridObject(currentTileX + dx, currentTileY + dz).type >= ElementType.Block;
        }


        //**********************************
        //TODO : remove debug
        testWallPivot = grid.GetGridObjectWorldCenter(currentTileX + posSignX, currentTileY + posSignZ);
        testWallDest = grid.GetGridObjectWorldCenter(currentTileX + dirSignX, currentTileY + dirSignZ);

        debugDir = Vector3.zero;
        debugPivot = Vector3.zero;
        //**********************************

        Vector3 moveDirection = travelDirection;
        dbgCol = Color.grey;//TODO : remove debug
        // no turn is possible here, exit the function
        if (blockingPivotTile && blockingDestTile && (posSignX != dirSignX && posSignZ != dirSignZ))
        {
            dbgCol = Color.yellow;//TODO : remove debug
            return position;
        }

        Vector3 newPosition = position;

        float cellSize = grid.GetCellsize();
        float radius = cellSize * 0.5f;

        // is the player input direction only on one axis (true) or diagonal (false)
        bool singleAxis = dirSignX != 0 ^ dirSignZ != 0;

        // calculate the corner position around which the player will turn
        Vector3 pivot = currentTileCenter + new Vector3(radius * posSignX, 0, radius * posSignZ);

        if (!blockingPivotTile && blockingDestTile)
        {
            dbgCol = Color.red; //TODO : remove debug
            // if the input direction (dirSign) is the same as the automatically detected direction (posSign)
            // then follow the detected direction (posSign), else the input direction has priority if it is not zero
            int signX = (posSignX == dirSignX || dirSignX == 0) ? posSignX : dirSignX;
            int signZ = (posSignZ == dirSignZ || dirSignZ == 0) ? posSignZ : dirSignZ;

            float absX = Mathf.Abs(offset.x);
            float absZ = Mathf.Abs(offset.z);

            // force the direction to follow the major axis
            if (singleAxis)
            {
                // auto sliding is active only when moving on a single axis against a corner
                moveDirection.x = signX * (absX > absZ ? (absX > cornerSliding ? 1 : 0) : 0);
                moveDirection.z = signZ * (absX < absZ ? (absZ > cornerSliding ? 1 : 0) : 0);
            }
            else
            {
                moveDirection.x = signX * (absX > absZ ? 1 : 0);
                moveDirection.z = signZ * (absX < absZ ? 1 : 0);
            }
        }
        else if (blockingPivotTile && !blockingDestTile)
        {
            dbgCol = Color.green; //TODO : remove debug

            // if the destination tile is free, set the movement vector in its direction
            Vector3 destination = grid.GetGridObjectWorldCenter(currentTileX + dirSignX, currentTileY + dirSignZ);

            moveDirection = (destination - newPosition);

            moveDirection.x = Utils.RoundedSign(moveDirection.x);
            moveDirection.z = Utils.RoundedSign(moveDirection.z);

            moveDirection.x = blockingX ? (posSignX == dirSignX) ? 0 : moveDirection.x : moveDirection.x;
            moveDirection.z = blockingZ ? (posSignZ == dirSignZ) ? 0 : moveDirection.z : moveDirection.z;
        }
        //else if(blockingPivotTile && blockingDestTile && Mathf.Abs(dirSignX - posSignX) < 2 && Mathf.Abs(dirSignZ - posSignZ) < 2)
        else if (blockingPivotTile && blockingDestTile)
        {
            dbgCol = Color.blue; //TODO : remove debug
            // if both pivot and destination tiles are blocking
            // force the direction on the major axis (to prevent slowdown while moving in diagonal against walls)
            moveDirection.x = (blockingX ? (posSignX == dirSignX) ? 0 : dirSignX : dirSignX) * 1.05f; // add 5% to prevent locking when moving perfectly in diagonal against a corner
            moveDirection.z = blockingZ ? (posSignZ == dirSignZ) ? 0 : dirSignZ : dirSignZ;
        }
        else if (posSignX != 0 && posSignZ != 0)
        {
            // moving diagonally
            dbgCol = Color.cyan; //TODO : remove debug

            moveDirection.x = blockingX ? (posSignX == dirSignX) ? 0 : dirSignX : dirSignX;
            moveDirection.z = blockingZ ? (posSignZ == dirSignZ) ? 0 : dirSignZ : dirSignZ;
        }

        // compute the new position according to the defined direction
        newPosition = startPosition + moveDirection.normalized * travelDistance;

        //**********************************
        //TODO : remove debug
        intersect = newPosition;
        //**********************************


        // test if the new position is below the corners radius threshold
        float distance = Vector3.Distance(newPosition, pivot);
        if (distance < radius)
        {
            // push back the position on the corner radius
            Vector3 pushDirection = (newPosition - pivot).normalized;
            newPosition = (pivot + pushDirection * radius);

            // define the final position according to the allowed travel distance from the start point
            newPosition = startPosition + (newPosition - startPosition).normalized * travelDistance;
        }

        //**********************************
        //TODO : remove debug
        debugDir = moveDirection;
        debugPivot = pivot;
        //**********************************

        return newPosition;
    }

    public void Teleport(Vector3 position)
    {
        if (isServer) {
            networkTransform.ServerTeleport(position);
            RpcUpdateTileCoordinates();
        }
        else
        {
            transform.position = position;
            UpdateTileCoordinates();
        }
    }
}

