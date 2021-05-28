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
            }
        }

        
       
    }

    private void Move(float h, float v)
    {
        UpdateTileCoordinates();
        ComputeMovementLimits();

        lastPosition = transform.position;

        Vector3 dir = new Vector3(h, 0, v);
        lastPosition += dir.normalized * (Time.deltaTime * speed);

        lastPosition.x = Mathf.Clamp(lastPosition.x, movementClampLow.x, movementClampHigh.x);
        lastPosition.z = Mathf.Clamp(lastPosition.z, movementClampLow.z, movementClampHigh.z);

        transform.position = lastPosition;
    }

    private void OnDrawGizmos()
    {
        if (LevelBuilder.grid != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(currentTileCenter, Vector3.one*0.95f);
        }
        
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

}

