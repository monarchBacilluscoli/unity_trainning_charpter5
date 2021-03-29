using UnityEngine;

/// <summary>
/// 可存档物体的信息存档机制
/// </summary>
[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    /// <summary>
    /// 存储当前物体
    /// </summary>
    public virtual void Save(GameDataWriter writer)
    {
        //todo 存储当前物体位置、旋转和缩放
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }

    /// <summary>
    /// 读取当前物体
    /// </summary>
    /// <param name="reader">存档读取器</param>
    public virtual void Load(GameDataReader reader)
    {
        //todo 读取当前物体位置、旋转和缩放
        transform.localPosition = reader.ReadVector3();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector3();
    }
}
