using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{

    #region Properties
    [SyncVar]bool isRunning;
    PlayerEntity playerEntity;

    public float speed = 5;
    public SO_Bool gameStarted;

    private int currentTileX;
    private int currentTileY;

    private Vector3 currentTileCenter;
    private Vector3 movementClampLow;
    private Vector3 movementClampHigh;
    private Vector3 lastPosition;

    private Tile majorTile;
    private Tile minorTile;
    #endregion



    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
        lastPosition = transform.position;
    }

    public void SetWorldPosition(Vector3 position)
    {
        transform.position = position;
        UpdateTileCoordinates();
        ComputeMovementLimits();
    }
    

    void Update()
    {
        if (!gameStarted.value) return;

        if (playerEntity.hubIdentity.isLocalPlayer)
        {
            if (playerEntity.controllerDevice == null || !Application.isFocused) return;

            // 
            Vector2 moveVector = playerEntity.controllerDevice.inputs.GetMoveVector();

            float horizontal = moveVector.x;
            float vertical = moveVector.y;

            if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 0.1f)
            {
                Move(horizontal, vertical);
                isRunning = true;
                playerEntity.animator.SetBool("IsRunning", isRunning);
            }
            else
            {
                isRunning = false;
                playerEntity.animator.SetBool("IsRunning", isRunning);
            }

        }
        else
        {
            Vector3 difference = transform.position - lastPosition;
            if (Mathf.Abs(difference.x) + Mathf.Abs(difference.z) > 0.01f)
            {
                UpdateTileCoordinates();
                lastPosition = transform.position;
                isRunning = true;
                playerEntity.animator.SetBool("IsRunning", isRunning);
            }
            else if (isRunning)
            {
                isRunning = false;
                playerEntity.animator.SetBool("IsRunning", isRunning);
            }
        }
    }

    private void Move(float h, float v)
    {
        UpdateTileCoordinates();
        ComputeMovementLimits();

        lastPosition = transform.position;
        Vector3 newPosition = lastPosition;
        float travelDistance = (Time.deltaTime * speed);
        Vector3 dir = new Vector3(h, 0, v);
        newPosition += dir.normalized * travelDistance;

        newPosition = ComputeCorners(newPosition, dir , travelDistance);

        newPosition.x = Mathf.Clamp(newPosition.x, movementClampLow.x, movementClampHigh.x);
        newPosition.z = Mathf.Clamp(newPosition.z, movementClampLow.z, movementClampHigh.z);

        transform.position = newPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (LevelBuilder.grid != null)
        {
            
            Gizmos.DrawWireCube(currentTileCenter, Vector3.one*0.95f);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(debugPivot, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + debugDir);

       
        Gizmos.color = pivotWall ? Color.red : Color.green;
        Gizmos.DrawWireSphere(testWallPivot, 0.49f);

        Gizmos.color = destWall ? Color.magenta : Color.yellow;
        Gizmos.DrawWireSphere(testWallDest, 0.4f);
    }

    private void UpdateTileCoordinates()
    {
        int tilex, tiley;

        LevelBuilder.grid.GetXY(transform.position, out tilex, out tiley);

        if(currentTileX != tilex || currentTileY != tiley)
        {
            if(majorTile != null && majorTile.type == ElementType.Player)
            {
                majorTile.SetType(ElementType.Empty);
            }

            Vector3 worldPosition = transform.position;
            majorTile = LevelBuilder.grid.GetGridObject(worldPosition);

            
            if (majorTile != null)
            {
                currentTileX = majorTile.x;
                currentTileY = majorTile.y;
                if(majorTile.type == ElementType.Empty)
                {
                    majorTile.SetType(ElementType.Player);
                }
                currentTileCenter = LevelBuilder.grid.GetGridObjectWorldCenter(currentTileX, currentTileY);
            }

        }
    }

    private void ComputeMovementLimits()
    {

        bool leftWall = LevelBuilder.grid.GetGridObject(currentTileX - 1, currentTileY).type >= ElementType.Block;
        bool rightWall = LevelBuilder.grid.GetGridObject(currentTileX + 1, currentTileY).type >= ElementType.Block;

        bool upWall = LevelBuilder.grid.GetGridObject(currentTileX, currentTileY + 1).type >= ElementType.Block;
        bool downWall = LevelBuilder.grid.GetGridObject(currentTileX, currentTileY - 1).type >= ElementType.Block;

        float cellSize = LevelBuilder.grid.GetCellsize();

        Vector3 minOffset = new Vector3(leftWall ? 0 : -cellSize, 0 , downWall ? 0 : -cellSize);
        Vector3 maxOffset = new Vector3(rightWall ? 0 : cellSize, 0, upWall ? 0 : cellSize);

        movementClampLow = currentTileCenter + minOffset;
        movementClampHigh = currentTileCenter + maxOffset;
    }

    Vector3 debugPivot;
    Vector3 debugDir;
    Vector3 testWallPivot;
    Vector3 testWallDest;

    bool pivotWall;
    bool destWall;
    private Vector3 ComputeCorners(Vector3 position , Vector3 travelDirection , float travelDistance)
    {
        Vector3 offset = position - currentTileCenter;

        int posSignX = Utils.RoundedSign(offset.x);
        int posSignZ = Utils.RoundedSign(offset.z);

        int dirSignX = Utils.RoundedSign(travelDirection.x);
        int dirSignZ = Utils.RoundedSign(travelDirection.z);

        bool wallPivot = LevelBuilder.grid.GetGridObject(currentTileX + posSignX, currentTileY + posSignZ).type >= ElementType.Block;
        bool wallDest = LevelBuilder.grid.GetGridObject(currentTileX + dirSignX, currentTileY + dirSignZ).type >= ElementType.Block;

        

        testWallPivot = LevelBuilder.grid.GetGridObjectWorldCenter(currentTileX + posSignX, currentTileY + posSignZ);
        testWallDest = LevelBuilder.grid.GetGridObjectWorldCenter(currentTileX + dirSignX, currentTileY + dirSignZ);

        debugDir = Vector3.zero;
        debugPivot = Vector3.zero;
        pivotWall = wallPivot;
        destWall = wallDest;

        debugDir = travelDirection;
        if (wallPivot && wallDest && (posSignX != dirSignX && posSignZ != dirSignZ) ) return position;

        Vector3 newPosition = position;
        
        if (posSignX != 0 && posSignZ != 0)
        {
            float cellSize = LevelBuilder.grid.GetCellsize();
            float radius = cellSize * 0.5f;
            
            Vector3 pivot = currentTileCenter + new Vector3(radius * posSignX, 0, radius * posSignZ);

            float distance = Vector3.Distance(position, pivot);
            Vector3 moveDirection = travelDirection;

            if (!pivotWall && destWall)
            {
                moveDirection.x = posSignX * (Mathf.Abs(offset.x) > Mathf.Abs(offset.z) ? 4 : 1);
                moveDirection.z = posSignZ * (Mathf.Abs(offset.x) < Mathf.Abs(offset.z) ? 4 : 1);

            }else if(pivotWall && !destWall)
            {
                Vector3 destination = LevelBuilder.grid.GetGridObjectWorldCenter(currentTileX + dirSignX, currentTileY + dirSignZ);

                moveDirection = (destination - newPosition);
            }


            if (distance < radius)
            {
                Vector3 pushDirection = (position - pivot).normalized;

                newPosition = pivot + pushDirection * radius;

                moveDirection = (newPosition - position);
            }


            debugDir = moveDirection;
            newPosition = position + moveDirection.normalized * travelDistance;

            debugPivot = pivot;
        }
        
        return newPosition;
    }
}

