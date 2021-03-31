using UnityEngine;

/// <summary>
/// 组合生成空间
/// </summary>
public class CompositeSpawnZone : SpawnZone
{
    #region 设定参数

    /// <summary>
    /// 是否采用顺序遍历的方式创建对象
    /// </summary>
    [SerializeField]
    bool m_sequential;

    /// <summary>
    /// 所有创生空间的列表
    /// </summary>
    [SerializeField]
    SpawnZone[] m_spwanZones;

    #endregion // 设定参数

    #region 中间变量
    int m_nextSequentialIndex;
    #endregion

    /// <summary>
    /// 生成物体的位置
    /// </summary>
    /// <value>生成位置</value>
    public override Vector3 SpawnPoint
    {
        get
        {
            int index;
            if (m_sequential)
            {
                index = m_nextSequentialIndex++;
                if (m_nextSequentialIndex >= m_spwanZones.Length)
                {
                    m_nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, m_spwanZones.Length);
            }
            return m_spwanZones[index].SpawnPoint;
        }
    }

    /// <summary>
    /// 存储当前对象
    /// </summary>
    /// <param name="writer">写入器</param>
    public override void Save(GameDataWriter writer)
    {
        // 保留下一个要创生的物体的index
        writer.Write(m_nextSequentialIndex);
    }

    /// <summary>
    /// 读取当前对象
    /// </summary>
    /// <param name="reader">读取器</param>
    public override void Load(GameDataReader reader)
    {
        // 读取下一个要创生的物体的index
        m_nextSequentialIndex = reader.ReadInt();
    }
}
