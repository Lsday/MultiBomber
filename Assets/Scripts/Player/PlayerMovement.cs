using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SyncVar] public NetworkIdentity hubIdentity;

    public byte playerIndex;
    public float speed = 5;
    [SyncVar] public byte localPlayerIndex;

    Animator animator;
    [SyncVar]bool isRunning;

    public PlayerInputs inputs;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (inputs == null && hubIdentity.isLocalPlayer)
        {
            inputs = PlayerInputs.instances[localPlayerIndex];
        }
    }

    public void SetHubIdentity(NetworkIdentity identity)
    {
        hubIdentity = identity;
    }

    public void SetLocalPlayerIndex(byte index)
    {
        localPlayerIndex = index;
    }

    void Update()
    {
        
        if (!hubIdentity.isLocalPlayer || inputs == null || !Application.isFocused) return;

        Vector2 moveVector = inputs.GetMoveVector();

        float horizontal = moveVector.x;
        float vertical = moveVector.y;

        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 0.1f)
        {

            Move(horizontal, vertical);
            isRunning = true;
            animator.SetBool("IsRunning", isRunning);
        }
        else
        {
            isRunning = false;
            animator.SetBool("IsRunning", isRunning);
        }
       
    }

    private void Move(float h, float v)
    {
        Vector3 dir = new Vector3(h, 0, v);
        transform.position += dir.normalized * (Time.deltaTime * speed);
    }

}

