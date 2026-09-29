using UnityEngine;

// 초기화·정리할 상태가 없어 ISubManager(Init·Clear)를 구현하지 않는다
public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if(index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if(go != null)
                return go as T; 
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>(path);

        if(prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if(prefab.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(prefab, parent);

        return Object.Instantiate(prefab, parent);
    }

    public void Destroy(GameObject go)
    {
        if(go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
