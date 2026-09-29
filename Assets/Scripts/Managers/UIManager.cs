using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ISubManager
{
    private int _order = 10;
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private UI_Scene _sceneUI = null;
    public UI_Scene SceneUI => _sceneUI;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void Init() { }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
            canvas = go.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);

        T sceneUI = go.GetComponent<T>();
        if (sceneUI == null)
            sceneUI = go.AddComponent<T>();

        _sceneUI = sceneUI;
        go.transform.SetParent(Root.transform);

        SetCanvas(go, sort: false);
        sceneUI.Init();

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);

        T popup = go.GetComponent<T>();
        if (popup == null)
            popup = go.AddComponent<T>();

        _popupStack.Push(popup);
        go.transform.SetParent(Root.transform);

        SetCanvas(go);
        popup.Init();

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        // 맨 위 팝업만 닫는다. 중간 팝업을 Destroy하면 스택에 파괴된 참조가 남는다
        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
        _order = 10;
    }
}