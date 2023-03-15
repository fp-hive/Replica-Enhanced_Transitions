using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();
            return _instance;
        }
    }

    static T _instance;

    public static bool instanceExists
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            _instance = null;
        }
    }
}