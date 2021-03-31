using UnityEngine;

/// <summary>
/// 一个会旋转的物体:>
/// </summary>
public class RotatingObject : MonoBehaviour
{
    /// <summary>
    /// 角速度
    /// </summary>
    [SerializeField]
    Vector3 m_angularVelocity;

    /// <summary>
    /// 更新物体角度
    /// </summary>
    void FixedUpdate()
    {
        transform.Rotate(m_angularVelocity * Time.deltaTime);
    }
}
