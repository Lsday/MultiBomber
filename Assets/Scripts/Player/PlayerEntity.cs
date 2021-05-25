using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerEntity : NetworkBehaviour
{
    public static List<PlayerEntity> instancesList = new List<PlayerEntity>();
    public static void SortInstances()
    {
        instancesList.Sort((x, y) => {
            var ret = x.hubIdentity.netId.CompareTo(y.hubIdentity.netId);
            if (ret == 0) ret = x.localPlayerIndex.CompareTo(y.localPlayerIndex);
            return ret;
        });
    }

    public static int GetInstancesCount()
    {
        return instancesList.Count;
    }

    [SyncVar] public string debugName;
    [SyncVar] public NetworkIdentity hubIdentity;
    [SyncVar] public byte localPlayerIndex;
    [SyncVar] public Color defaultColor;

    //public byte playerIndex;

    public DeviceEntity controllerDevice;
    public Animator animator;
    public Renderer avatarRenderer;
    public Material avatarMaterial;



    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        avatarRenderer = GetComponentInChildren<Renderer>();
        avatarMaterial = avatarRenderer.material;
    }

    private void Start()
    {

        if (hubIdentity.isLocalPlayer)
        {
            if (controllerDevice == null)
            {
                controllerDevice = DeviceEntity.instancesList[localPlayerIndex];
            }
            SortInstances();
        }

        if (isServer)
        {
            defaultColor = Random.ColorHSV();
            RpcSetColor(defaultColor);
        }

        ApplyColor();


    }

    [ClientRpc]
    void RpcSetColor(Color newColor)
    {
        defaultColor = newColor;
        ApplyColor();
    }

    void ApplyColor()
    {
        avatarMaterial.color = defaultColor;
    }

    public void SetHubIdentity(NetworkIdentity identity)
    {
        hubIdentity = identity;
    }

    public void SetLocalPlayerIndex(byte index)
    {
        localPlayerIndex = index;
    }

    private void OnEnable()
    {
        instancesList.Add(this);
        if(hubIdentity) SortInstances();
    }

    private void OnDisable()
    {
        instancesList.Remove(this);
        if (hubIdentity) SortInstances();
    }
}
