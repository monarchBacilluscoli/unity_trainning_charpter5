using UnityEngine;

/// <summary>
/// 形状
/// </summary>
public class Shape : PersistableObject
{
    #region 设定变量
    static int m_colorPropertyId = Shader.PropertyToID("_Color");
    #endregion //设定变量

    #region 中间变量

    /// <summary>
    /// 记录当前Shape的Id也就是类型
    /// </summary>
    int m_shapeId = int.MinValue; //MinValue是一个字面值

    /// <summary>
    /// 形状id
    /// </summary>
    /// <value>形状id</value>
    public int ShapeId
    {
        get
        {
            return m_shapeId;
        }
        set
        {
            if (m_shapeId == int.MinValue && value != int.MinValue)
            {
                m_shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapedID");
            }
        }
    }

    /// <summary>
    /// 材质Id
    /// </summary>
    /// <value>材质id</value>
    public int MaterialId
    {
        get;
        private set;
    }

    /// <summary>
    /// 当前物体颜色
    /// </summary>
    Color m_color;

    /// <summary>
    /// 网格渲染器
    /// </summary>
    MeshRenderer m_meshRenderer;

    static MaterialPropertyBlock s_sharedPropertyBlock;

    #endregion // 中间变量

    /// <summary>
    /// 当初始化时被调用
    /// </summary>
    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 存档
    /// </summary>
    /// <param name="writer">写入器</param>
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(m_color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }

    /// <summary>
    /// 设置材质及材质id
    /// </summary>
    /// <param name="material">材质</param>
    /// <param name="materialId">材质Id</param>
    public void SetMaterial(Material material, int materialId)
    {
        m_meshRenderer.material = material;
        MaterialId = materialId;
    }

    /// <summary>
    /// 设定颜色
    /// </summary>
    /// <param name="color">待设定颜色</param>
    public void SetColor(Color color)
    {
        m_color = color;
        if (s_sharedPropertyBlock == null)
        {
            s_sharedPropertyBlock = new MaterialPropertyBlock();
        }
        s_sharedPropertyBlock.SetColor(m_colorPropertyId, color);
        m_meshRenderer.SetPropertyBlock(s_sharedPropertyBlock);
    }
}
