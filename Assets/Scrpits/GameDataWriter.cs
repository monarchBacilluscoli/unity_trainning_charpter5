using System.IO;
using UnityEngine;

/// <summary>
/// 存档工具
/// </summary>
public class GameDataWriter
{
    BinaryWriter m_writer;

    public GameDataWriter(BinaryWriter writer)
    {
        m_writer = writer;
    }

    /// <summary>
    /// 写入flaot值
    /// </summary>
    /// <param name="value">待写入的值</param>
    public void Write(float value)
    {
        m_writer.Write(value);
    }

    /// <summary>
    /// 写入int值
    /// </summary>
    /// <param name="value">待写入的值</param>
    public void Write(int value)
    {
        m_writer.Write(value);
    }

    /// <summary>
    ///  写入旋转值
    /// </summary>
    /// <param name="value">待写入的值</param>
    public void Write(Quaternion value)
    {
        m_writer.Write(value.x);
        m_writer.Write(value.y);
        m_writer.Write(value.z);
        m_writer.Write(value.w);
    }

    /// <summary>
    /// 写入Vector3
    /// </summary>
    /// <param name="value">待写入的值</param>
    public void Write(Vector3 value)
    {
        m_writer.Write(value.x);
        m_writer.Write(value.y);
        m_writer.Write(value.z);
    }

    /// <summary>
    /// 写入颜色值
    /// </summary>
    /// <param name="value">颜色值</param>
    public void Write(Color value)
    {
        m_writer.Write(value.r);
        m_writer.Write(value.g);
        m_writer.Write(value.b);
        m_writer.Write(value.a);
    }
}
