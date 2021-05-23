using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkIdentity hubIdentity;
    private PlayersHub myHub;
    public byte playerIndex;
    public float speed = 5;
    public KeyCode myKey;

    Animator animator;
    [SyncVar]bool isRunning;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(NetworkIdentity identity , byte index, KeyCode key)
    {
        playerIndex = index;
        hubIdentity = identity;
        myHub = hubIdentity.GetComponent<PlayersHub>();
        transform.position += transform.forward * index;
        myKey = key;

        myHub.LinkPlayer(this);
    }

    public void SetMovement(Vector2 movement)
    {
        float horizontal = movement.x;
        float vertical = movement.y;

        if (horizontal + vertical !=0)
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

