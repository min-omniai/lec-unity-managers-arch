using UnityEngine;
using System.Collections.Generic;

public class PoolManager : ISubManager
{
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        private Stack<Poolable> _poolStack = new Stack<Poolable>();
        
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable == null)
                poolable = go.AddComponent<Poolable>();

            return poolable;
        }

        public void Push(Poolable poolable)
        {
            if(poolable == null)
                return;

            poolable.OnPushToPool();
            poolable.transform.SetParent(Root);
            poolable.gameObject.SetActive(false);
            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable = _poolStack.Count > 0 ? _poolStack.Pop() : Create();
            poolable.transform.SetParent(parent);
            poolable.gameObject.SetActive(true);
            poolable.OnPopFromPool();

            return poolable;
        }
    }

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root = null;

    public void Init()
    {
        _root = new GameObject { name = "@PoolRoot" }.transform;
        Object.DontDestroyOnLoad(_root);

        // 테스트 프리팹 풀 미리 생성 (Resources/Characters)
        CreatePool(Resources.Load<GameObject>("Characters/Cube"));
        CreatePool(Resources.Load<GameObject>("Characters/Sphere"));
        CreatePool(Resources.Load<GameObject>("Characters/Capsule"));
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        // 이미 있으면 스킵
        if (_pool.ContainsKey(original.name))
            return;  

        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.SetParent(_root);
        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if(!_pool.ContainsKey(name))
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }

    public GameObject Pop(GameObject original, Transform parent = null)
    {
        if(!_pool.ContainsKey(original.name))
            CreatePool(original);

        return _pool[original.name].Pop(parent).gameObject;
    }

    public GameObject GetOriginal(string name)
    {
        if(!_pool.ContainsKey(name))
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        if(_root == null)
            return;

        foreach (Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);
        }

        _pool.Clear();
    }
}