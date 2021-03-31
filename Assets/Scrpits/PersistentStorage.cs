using UnityEngine;
using System.IO;

/// <summary>
/// 用于对可存档物体进行统一存档
/// </summary>
public class PersistentStorage : MonoBehaviour
{
    /// <summary>
    /// 存档的位置
    /// </summary>
    string m_savePath;

    void Awake()
    {
        // 初始化路径
        m_savePath = Path.Combine(Application.persistentDataPath, "savePath");
    }

    /// <summary>
    /// 存储可存储物体
    /// </summary>
    /// <param name="o">可存储物体组件</param>
    /// <remarks>
    /// 在这里创建Writer并调用物体o的<c>Save()</c>方法
    /// </remarks>
    public void Save(PersistableObject o, int version)
    {
        using (var writer = new BinaryWriter(File.Open(m_savePath, FileMode.Create)))
        {
            writer.Write(-version);
            o.Save(new GameDataWriter(writer));
        }
    }

    /// <summary>
    /// 读取可存储的个体
    /// </summary>
    /// <param name="o">待读取的个体</param>
    public void Load(PersistableObject o)
    {
        // 将所有数据读到内存数组
        byte[] data = File.ReadAllBytes(m_savePath);
        // 创建内存流用数组初始化，并构建基于内存的BinaryReader
        var reader = new BinaryReader(new MemoryStream(data));
        // 按原样应用存档数据
        o.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}
