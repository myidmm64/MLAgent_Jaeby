using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public PoolType poolType = PoolType.None;

    public abstract void StartInit();
    public abstract void PopInit();
    public abstract void PushInit();
}
