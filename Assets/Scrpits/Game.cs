using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//! 不要把它看成manager，它就是game! Save它就是save game!
/// <summary>
/// 控制简单的物体生成+存档的逻辑
/// </summary>
public class Game : PersistableObject
{
    #region 设置

    /// <summary>
    /// 创生拉条
    /// </summary>
    [SerializeField]
    Slider m_creationSpeedSlider;

    /// <summary>
    /// 破坏拉条
    /// </summary>
    [SerializeField]
    Slider m_destructionSpeedSlider;

    /// <summary>
    /// 游戏在每次开始时是否将随机数重置
    /// </summary>
    [SerializeField] bool m_reseedOnLoad;

    /// <summary>
    /// 创生位置的property
    /// </summary>
    /// <remarks>
    /// 用于其他scene设定该值
    /// </remarks>
    public SpawnZone SpawnZoneOfLevel
    {
        get;
        set;
    }

    [SerializeField]
    ShapeFactory m_shapeFactory;

    /// <summary>
    /// 生成物体的按键
    /// </summary>
    [SerializeField]
    KeyCode m_createKey = KeyCode.C;

    /// <summary>
    /// 开始新游戏的按键
    /// </summary>
    [SerializeField]
    KeyCode m_newGameKey = KeyCode.N;

    /// <summary>
    /// 游戏存档按键
    /// </summary>
    [SerializeField]
    KeyCode m_saveKey = KeyCode.S;

    /// <summary>
    /// 读取存档按键
    /// </summary>
    [SerializeField]
    KeyCode m_loadKey = KeyCode.L;

    /// <summary>
    /// 销毁物体的按键
    /// </summary>
    [SerializeField]
    KeyCode m_destoryKey = KeyCode.X;

    /// <summary>
    /// 存档管理器
    /// </summary>
    [SerializeField]
    PersistentStorage m_storage;

    /// <summary>
    /// 存档版本
    /// </summary>
    /// <remarks>
    /// 随存档格式更新++
    /// </remarks>
    const int m_saveVersion = 3;

    /// <summary>
    /// 关卡总数
    /// </summary>
    [SerializeField]
    int m_levelCount;

    #endregion // 设置

    #region 中间变量

    /// <summary>
    /// 当前游戏的主随机状态
    /// </summary>
    Random.State m_mainRandomState;

    /// <summary>
    /// 当前关卡的build index
    /// </summary>
    int loadedLevelBuildIndex;

    /// <summary>
    /// 记录所有对象的位置
    /// </summary>
    List<Shape> m_shapes;

    /// <summary>
    /// 物体生成个数 per second
    /// </summary>
    /// <value>速度值</value>
    public float CreationSpeed
    {
        get;
        set;
    }

    /// <summary>
    /// 销毁物体个数 per second
    /// </summary>
    /// <value>速度值</value>
    public float DestructionSpeed
    {
        get;
        set;
    }


    /// <summary>
    /// 当前物体的创建进程
    /// </summary>
    /// <remarks>
    /// 随时间推移增加，当达到1会摧毁旧物体
    /// </remarks>
    float m_creationProgress;

    /// <summary>
    /// 当前物体的创建进程
    /// </summary>
    /// <remarks>
    /// 随时间推移增加，当达到1会摧毁旧物体
    /// </remarks>
    float m_destructionProgress;

    #endregion // 中间变量

    /// <summary>
    /// 初始化时被调用
    /// </summary>
    void Start()
    {
        // 初始化记录列表
        m_shapes = new List<Shape>();
        m_levelCount = SceneManager.sceneCountInBuildSettings - 1;

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level"))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }
        BeginNewGame();
        // 开启异步任务
        StartCoroutine(LoadLevel(1));
    }

    /// <summary>
    /// 每帧被调用
    /// </summary>
    void Update()
    {
        // 如果按下创建物体按键，则创建物体
        if (Input.GetKey(m_createKey))
        {
            CreateShape();
        }
        // 如果按下NewGame按键，开始新游戏
        else if (Input.GetKeyDown(m_newGameKey))
        {
            // 清空先前痕迹
            BeginNewGame();
            // 开始新游戏
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
        }
        else if (Input.GetKeyDown(m_saveKey))
        {
            // 存储Game！存储this，就是存储Game！
            m_storage.Save(this, m_saveVersion);
        }
        else if (Input.GetKeyDown(m_loadKey))
        {
            // 刷新游戏状态
            BeginNewGame();
            // 读取Game！读取this，就是读取Game！
            m_storage.Load(this);
        }
        else if (Input.GetKey(m_destoryKey))
        {
            DestroyShape();
        }
        else
        {
            // 如果按下数字键，则加载对应index的关卡
            for (int i = 0; i <= m_levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    StartCoroutine(LoadLevel(i));
                    BeginNewGame();
                    return;
                }
            }
        }
    }
    void FixedUpdate()
    {
        // 时间到，创生
        m_creationProgress += Time.deltaTime * CreationSpeed;
        while (m_creationProgress >= 1.0f)
        {
            m_creationProgress -= 1f;
            CreateShape();
        }
        // 时间到，毁灭
        m_destructionProgress += Time.deltaTime * DestructionSpeed;
        while (m_destructionProgress >= 1f)
        {
            m_creationProgress -= 1f;
            DestroyShape();
        }
    }

    /// <summary>
    /// 用于创建物体
    /// </summary>
    void CreateShape()
    {
        // 创建物体并修改位置、旋转和缩放
        Shape instance = m_shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = GameLevel.Current.SpawnPoint;
        t.rotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        // 设置颜色
        instance.SetColor(Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f));

        // 记录物体
        m_shapes.Add(instance);
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    /// <remarks>
    /// 负责先前物体的清理工作
    /// </remarks>
    void BeginNewGame()
    {
        Random.state = m_mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        m_mainRandomState = Random.state;
        Random.InitState(seed);
        m_creationSpeedSlider.value = 0f;
        m_destructionSpeedSlider.value = 0f;
        // 摧毁所有记录的对象
        for (int i = 0; i < m_shapes.Count; i++)
        {
            m_shapeFactory.Reclaim(m_shapes[i]);
        }
        // 删除所有记录
        m_shapes.Clear();
    }

    /// <summary>
    /// 存储游戏状态
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <remarks>
    /// 写入这个就是写入游戏！
    /// </remarks>
    public override void Save(GameDataWriter writer)
    {
        // 存储存档版本
        writer.Write(m_shapes.Count);
        // 写入当前随机数状态
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(m_creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(m_destructionProgress);
        // 写入关卡build编号
        writer.Write(loadedLevelBuildIndex);
        // 写入当前关卡状态
        GameLevel.Current.Save(writer);
        for (int i = 0; i < m_shapes.Count; i++)
        {
            // 存储shapeId
            writer.Write(m_shapes[i].ShapeId);
            writer.Write(m_shapes[i].MaterialId);
            m_shapes[i].Save(writer);
        }
    }

    /// <summary>
    /// 读取存档
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <remarks>
    /// 读取这个就是读取游戏！
    /// </remarks>
    public override void Load(GameDataReader reader)
    {
        // 读取存档版本
        int version = reader.Version;
        // 检查是否存档版本大于当前版本
        if (version > m_saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }
        // load整个关卡
        StartCoroutine(LoadGame(reader)); // 这里使用协程，但是外部传递进来的包装了BinaryReader的GameDataReader中的binaryReader是using的，在退栈的时候就会挂掉。所以这里使用异步（异处）会出问题
    }

    /// <summary>
    /// 读取整个游戏
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <returns>协程使用的枚举器</returns>
    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        // 检查是否为存档版本
        int count = version < 0 ? -version : reader.ReadInt();
        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!m_reseedOnLoad)
            {
                Random.state = state;
            }
            m_creationSpeedSlider.value = reader.ReadFloat();
            m_creationProgress = reader.ReadFloat();
            m_destructionSpeedSlider.value = reader.ReadFloat();
            m_destructionProgress = reader.ReadFloat();
        }
        // 开启新协程加载场景
        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3)
        {
            // 加载场景成功之后才能在当前场景包含的GameLevel更新后加载关卡的其他信息
            GameLevel.Current.Load(reader);
        }
        for (int s = 0; s < count; s++)
        {
            // 读取并设置shapId
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = m_shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            m_shapes.Add(instance);
        }
    }

    /// <summary>
    /// 毁掉某个物体
    /// </summary>
    void DestroyShape()
    {
        if (m_shapes.Count > 0)
        {
            int index = Random.Range(0, m_shapes.Count);
            m_shapeFactory.Reclaim(m_shapes[index]);
            // 将被distucted的玩意放到array最后并且
            int lastIndex = m_shapes.Count - 1;
            m_shapes[index] = m_shapes[lastIndex];
            m_shapes.RemoveAt(lastIndex);
        }
    }

    /// <summary>
    /// 异步加载特定build index的关卡
    /// </summary>
    /// <param name="levelBuildIndex">关卡的build index</param>
    /// <returns>枚举器，用于协程</returns>
    IEnumerator LoadLevel(int levelBuildIndex)
    {
        // 令该组件失效从而避免玩家在未加载时进行操作
        enabled = false;
        // 加载新场景前先关闭原有场景
        if (loadedLevelBuildIndex > 0)
        {
            SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        // 等待yield执行完毕耽搁执行后面的语句
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        // 当带有light的场景加载完毕，设定当前脚本起效
        enabled = true;
    }
}
