using UnityEngine;

/// <summary>
/// 物品生成点
/// </summary>
public abstract class SpawnZone : PersistableObject
{

    /// <summary>
    /// 物品的生成位置
    /// </summary>
    public abstract Vector3 SpawnPoint
    {
        get;
    }
}
