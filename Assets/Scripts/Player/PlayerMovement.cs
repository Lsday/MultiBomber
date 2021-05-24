using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SyncVar] public NetworkIdentity hubIdentity;

    public byte playerIndex;
    public float speed = 5;

    Animator animator;
    [SyncVar]bool isRunning;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetHubIdentity(NetworkIdentity identity)
    {
        hubIdentity = identity;
    }

    void Update()
    {
        if (!hubIdentity.isLocalPlayer) return;
    
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

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

