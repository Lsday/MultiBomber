using Mirror;

public interface IDestroyable
{
    public void Destroy();

    public void InitDestroy(float delay = 0f, float extraDelay = 0f);

    [ClientRpc]
    public void RpcInitDestroy(float delay, float endDelay);
}