using System.Collections;
using UnityEngine;

/// <summary>
/// A singleton that stays loaded for the life of the game.
/// If anybody attempts to access it, it's immediately force-loaded.
/// Duplicates are destroyed
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this as T;
        else
            Destroy(gameObject);
    }
}

/// <summary>
/// While an instance of an object exists, that instance will be quickly referenced.
/// </summary>
public class SoftSingleton<T> : MonoBehaviour where T : Component
{
    public static T instance;

    public virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
    }

    public virtual void OnDestroy()
    {
        if (object.ReferenceEquals(instance, this))
            instance = null;
    }
}
