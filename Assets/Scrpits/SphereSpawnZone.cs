using UnityEngine;

/// <summary>
/// 物品生成点
/// </summary>
public class SphereSpawnZone : SpawnZone
{
    /// <summary>
    /// 是否仅仅在球表面生成
    /// </summary>
    [SerializeField]
    bool m_surfaceOnly;

    /// <summary>
    /// 物品的生成位置
    /// </summary>
    public override Vector3 SpawnPoint // 可以重载property
    {
        get
        {
            return transform.TransformPoint(
                m_surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere
                );
        }
    }

    /// <summary>
    /// 绘制参考线
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}
