using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    
    [SyncVar]bool isRunning;
    public float speed = 5;

    PlayerEntity playerEntity;
    public SO_Bool gameStarted;

    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
    }

    void Update()
    {
        
        if (!gameStarted.value || !playerEntity.hubIdentity.isLocalPlayer || playerEntity.controllerDevice == null || !Application.isFocused) return;

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

    private void Move(float h, float v)
    {
        Vector3 dir = new Vector3(h, 0, v);
        transform.position += dir.normalized * (Time.deltaTime * speed);
    }

}

