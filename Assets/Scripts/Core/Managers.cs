using UnityEngine;

/*
@Managers (GameObject, DontDestroyOnLoad)
└── Managers.cs  ← 단일 진입점 싱글톤

서브 매니저들 (Plain C# Class, ISubManager)
├── ResourceManager
├── PoolManager
├── AudioManager
├── DataManager
└── UIManager
*/

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    AudioManager _audio = new AudioManager();
    ResourceManager _resource = new ResourceManager();
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    UIManager _ui = new UIManager();

    public static AudioManager Audio => Instance._audio;
    public static ResourceManager Resource => Instance._resource;
    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static UIManager UI => Instance._ui;

    static void Init()
    {
        if (_instance != null) return;

        // @Managers는 씬에 배치하지 않는다. GameScene(BaseScene.Awake)의 첫 접근에서 코드로 생성된다
        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject("@Managers");
            go.AddComponent<Managers>();

            _instance = go.GetComponent<Managers>();
            DontDestroyOnLoad(go.gameObject);

            Data.Init();
            Pool.Init();
            Audio.Init();
            UI.Init();
        }
    }

    public static void Clear()
    {
        Audio.Clear();
        Data.Clear();
        Pool.Clear();
        UI.Clear();
    }
}

