using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 用于生成Shape的asset
/// </summary>
[CreateAssetMenu] // 显示在创建物体菜单中
public class ShapeFactory : ScriptableObject
{
    #region 设置参数

    /// <summary>
    /// 是否对Shape进行回收
    /// </summary>
    [SerializeField]
    bool m_recycle;

    /// <summary>
    /// 待生成的prefab数组
    /// </summary>
    [SerializeField]
    Shape[] m_prefabs;

    /// <summary>
    /// 待生成prefab的材质
    /// </summary>
    [SerializeField]
    Material[] m_materials;

    #endregion // 设置参数

    #region 中间变量

    /// <summary>
    /// 用于存储
    /// </summary>
    Scene m_poolScene;

    /// <summary>
    /// 用于回收对象的回收池
    /// </summary>
    List<Shape>[] m_pools;

    #endregion

    /// <summary>
    /// 得到id位置的shape
    /// </summary>
    /// <param name="shapeId">形状id</param>
    /// <param name="materialId">材质id</param>
    /// <returns>创建的Shape</returns>
    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape instance;
        if (m_recycle)
        {
            if (m_pools == null)
            {
                CreatePools();
            }
            List<Shape> pool = m_pools[shapeId];
            int lastIndex = pool.Count - 1;
            //todo 如果对应的pool中有物体
            if (lastIndex >= 0)
            {
                //todo 提取pool中的最后一个物体并activate之
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                instance = Instantiate(m_prefabs[shapeId]);
                instance.ShapeId = shapeId;
                //todo 当需要真正创建新的对象的时候（初次创建）才会需要移动到别的scene
                SceneManager.MoveGameObjectToScene(
                    instance.gameObject, m_poolScene
                );
            }
        }
        else
        {
            //todo 生成对象
            instance = Instantiate(m_prefabs[shapeId]);
            //todo 设置ID
            instance.ShapeId = shapeId;
        }
        instance.SetMaterial(m_materials[materialId], materialId);
        return instance;
    }

    /// <summary>
    /// 得到随机Shape
    /// </summary>
    /// <returns>随机创建的Shape</returns>
    public Shape GetRandom()
    {
        return Get(Random.Range(0, m_prefabs.Length),
            Random.Range(0, m_materials.Length));
    }

    /// <summary>
    /// 创建回收池
    /// </summary>
    void CreatePools()
    {
        m_pools = new List<Shape>[m_prefabs.Length];
        for (int i = 0; i < m_pools.Length; i++)
        {
            m_pools[i] = new List<Shape>();
        }
        //todo 检查当前场景是否存在，用于处理recompilation
        if (Application.isEditor)
        {
            m_poolScene = SceneManager.GetSceneByName(name);
            if (m_poolScene.isLoaded)
            {
                //todo 获取所有的根对象，重建回收池（因为回收池并不进行序列化）
                GameObject[] rootObjects = m_poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    //todo 检查对象，添加到相对应的池中
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        m_pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }
        //todo 如果不存在方重建
        m_poolScene = SceneManager.CreateScene(name);
    }

    /// <summary>
    /// 回收指定Shape
    /// </summary>
    public void Reclaim(Shape shapeToRecycle)
    {
        if (m_recycle)
        {
            //todo 检查并新建回收池
            if (m_pools == null)
            {
                CreatePools();
            }
            //todo 加入待回收对象
            m_pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }
}
