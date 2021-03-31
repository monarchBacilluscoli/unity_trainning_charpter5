using UnityEngine;

/// <summary>
/// 物品生成点
/// </summary>
public class CubeSpawnZone : SpawnZone
{

    /// <summary>
    /// 是否仅生成在表面
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
            // 生成位于立方体内的形状
            Vector3 p;
            p.x = Random.Range(-0.5f, 0.5f);
            p.y = Random.Range(-0.5f, 0.5f);
            p.z = Random.Range(-0.5f, 0.5f);
            if (m_surfaceOnly)
            {
                // 随机某一个轴，并将该点置于与该轴正交的两个平面之一上
                int axis = Random.Range(0, 3);
                p[axis] = p[axis] > 0 ? 0.5f : -0.5f;
            }
            return transform.TransformPoint(p);
        }
    }

    /// <summary>
    /// 绘制参考立方体
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one); // 有前面的变换矩阵，这里只需要绘制单位cube即可
    }
}
