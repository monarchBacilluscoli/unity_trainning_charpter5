using UnityEngine;
using System.IO;

/// <summary>
/// 存档读取工具
/// </summary>
public class GameDataReader
{
    public int Version { get; }

    /// <summary>
    /// 二进制读取器
    /// </summary>
    BinaryReader m_reader;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="reader">该存档读取工具使用的BinaryReader</param>
    public GameDataReader(BinaryReader reader, int version)
    {
        m_reader = reader;
        Version = version;
    }

    /// <summary>
    /// 读取float值
    /// </summary>
    /// <returns>读取的float值</returns>
    public float ReadFloat()
    {
        return m_reader.ReadSingle();
    }

    /// <summary>
    /// 读取Int32值
    /// </summary>
    /// <returns>读取的Int32</returns>
    public int ReadInt()
    {
        return m_reader.ReadInt32();
    }

    /// <summary>
    /// 读取一个四元数
    /// </summary>
    /// <returns>读取的四元数</returns>
    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = m_reader.ReadSingle();
        value.y = m_reader.ReadSingle();
        value.z = m_reader.ReadSingle();
        value.w = m_reader.ReadSingle();
        return value;
    }

    /// <summary>
    /// 读取一个Vector3坐标
    /// </summary>
    /// <returns>读取到的Vector3坐标</returns>
    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = m_reader.ReadSingle();
        value.y = m_reader.ReadSingle();
        value.z = m_reader.ReadSingle();
        return value;
    }

    /// <summary>
    /// 读取颜色值
    /// </summary>
    /// <returns>读到的颜色</returns>
    public Color ReadColor()
    {
        Color value;
        value.r = m_reader.ReadSingle();
        value.g = m_reader.ReadSingle();
        value.b = m_reader.ReadSingle();
        value.a = m_reader.ReadSingle();
        return value;
    }

    /// <summary>
    /// 读取存档中的随机数状态
    /// </summary>
    /// <returns>存档中的随机状态</returns>
    public Random.State ReadRandomState()
    {
        // 读JsonUtility出来的string，并将之转换为Random.state
        return JsonUtility.FromJson<Random.State>(m_reader.ReadString());
    }
}
