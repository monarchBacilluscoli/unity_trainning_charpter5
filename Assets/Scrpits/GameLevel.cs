using UnityEngine;

/// <summary>
/// 管理关卡
/// </summary>
/// <remarks>
/// 设置创生位置
/// </remarks>
public class GameLevel : PersistableObject
{

    /// <summary>
    /// 创生区
    /// </summary>
    [SerializeField]
    SpawnZone m_spawnZone;

    /// <summary>
    /// 可存档物体
    /// </summary>
    [SerializeField]
    PersistableObject[] m_persistentObjects;

    /// <summary>
    /// 当前游戏关卡
    /// </summary>
    /// <value>当前GameLevel</value>
    /// <remarks>
    /// 作为游戏类的共有成员记录游戏关卡数据
    /// </remarks>
    public static GameLevel Current { get; private set; }

    /// <summary>
    /// 生效时调用
    /// </summary>
    void OnEnable()
    {
        Current = this;
        if (m_persistentObjects == null)
        {
            m_persistentObjects = new PersistableObject[0];
        }
    }

    /// <summary>
    /// 返回当前Level的创生点
    /// </summary>
    /// <value>创生坐标</value>
    public Vector3 SpawnPoint
    {
        get
        {
            return m_spawnZone.SpawnPoint;
        }
    }

    /// <summary>
    /// 存储当前关卡
    /// </summary>
    /// <param name="writer">写入器</param>
    public override void Save(GameDataWriter writer)
    {
        writer.Write(m_persistentObjects.Length);
        for (int i = 0; i < m_persistentObjects.Length; i++)
        {
            m_persistentObjects[i].Save(writer);
        }
    }

    /// <summary>
    /// 读取当前关卡
    /// </summary>
    /// <param name="reader">读取器</param>
    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            m_persistentObjects[i].Load(reader); //! 真的就load自己啊！
        }
    }
}
