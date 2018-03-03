using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 所有界面类的基类
/// </summary>
public abstract class BaseWindow
{
    protected Transform _transform;
    public string name;

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="wndName"></param>
    public void Open(Transform canvas, string wndName)
    {
        name = wndName;
        _transform = (GameObject.Instantiate(Resources.Load("UI/" + wndName)) as GameObject).transform;
        _transform.parent = canvas;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
        _transform.name = wndName;
    }
    public virtual void Initialize() { }
    /// <summary>
    /// 关闭窗口
    /// </summary>
    public virtual void Close()
    {
        if (_transform != null && _transform.gameObject != null)
            GameObject.Destroy(_transform.gameObject);
    }

    public virtual void Update(float dt) { }
}
class UIManager : SingleTon<UIManager>
{
    public Transform _canvas;

    // 保存所有的打开的窗口
    public Dictionary<string, BaseWindow> _windows = new Dictionary<string, BaseWindow>();

    public Camera uiCamera;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        MonoBehaviour.DontDestroyOnLoad(GameObject.Find("UI"));
        _canvas = GameObject.Find("UI/Canvas").transform;
        uiCamera = GameObject.Find("UI/Camera").GetComponent<Camera>();
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Open<T>() where T : BaseWindow, new()
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            return _windows[wndName] as T;
        }
        else
        {
            T wnd = new T();
            wnd.Open(_canvas, wndName);
            _windows.Add(wndName, wnd);
            Debug.Log("wndName:"+ wndName);
            return wnd;
        }
    }

    /// <summary>
    /// 关闭窗口 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Close<T>() where T : BaseWindow
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            _windows[wndName].Close();
            _windows.Remove(wndName);
        }
    }

    public T Get<T>() where T : BaseWindow
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            return _windows[wndName] as T;
        }
        else
        {
            return default(T);
        }
    }

    public void UIUpdate(float dt)
    {
        foreach (BaseWnd wnd in _windows.Values)
        {
            wnd.Update(dt);
        }
    }
}
public class SingleTon<T> where T : class, new()
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }
}
