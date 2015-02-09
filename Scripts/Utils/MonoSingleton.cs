using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : class
{
    public const string GameObjectName = "Singletons";

    private static T _instance;
    private static GameObject _parent;
    private static bool _isRunning = true;
    private static bool _isInstantiating = false;

    protected static bool IsRunning
    {
        get { return _isRunning; }
    }

    private static GameObject Parent
    {
        get
        {
            _parent = GameObject.Find(GameObjectName);
            if (_parent == null)
            {
                _parent = new GameObject(GameObjectName);
                DontDestroyOnLoad(_parent);
            }
            return _parent;
        }
    }

    private void OnApplicationQuit()
    {
        _isRunning = false;
    }

    protected virtual void Start()
    {
        //this line kicks in, if the script was dragged on to a gameobject at design time
        if (_instance == null)
        {
            throw new Exception("The script " + typeof(T).Name + " is self instantiating and shouldn't be attached manually to a GameObject.");
        }
    }

    public static T Instance
    {
        get
        {
            if (_instance == null && _isRunning)
            {
                if (_isInstantiating)
                {
                    throw new Exception("Recursive calls to Constuctor of MonoSingleton! Check your " + typeof(T) + ":Awake() function for calls to " + typeof(T) + ".Instance");
                }
                else
                {
                    _isInstantiating = true;
                    _instance = Parent.AddComponent(typeof(T)) as T;
                }
            }
            return _instance;
        }
    }
}