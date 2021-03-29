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
        using (var reader = new BinaryReader(File.Open(m_savePath, FileMode.Open)))
        {
            // 在进行任何读取存档操作前，先读取版本号并传递给reader
            o.Load(new GameDataReader(reader, -reader.ReadInt32()));
        }
    }
}
