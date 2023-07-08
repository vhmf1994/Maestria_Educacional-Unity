using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                    instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            }

            return instance;
        }
        protected set
        {
            instance = value;
        }
    }

    [SerializeField] private bool dontDestroyOnLoad = false;

    private void Awake()
    {
        if (instance != this)
            DestroySingleton();

        instance = Instance as T;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        UpdateBehaviour();
    }

    /// <summary>
    /// Initialize - Initialize the singleton object
    /// </summary>
    /// <returns></returns>
    protected virtual void InitializeBehaviour() { }
    /// <summary>
    /// Update - Updates the singleton object
    /// </summary>
    /// <returns></returns>
    protected virtual void UpdateBehaviour() { }

    /// <summary>
    /// Finalize - Finalize the singleton object
    /// </summary>
    /// <returns></returns>
    protected virtual void FinishBehaviour() { }

    /// <summary>
    /// DestroySingleton - Destroy the singleton object
    /// </summary>
    public static void DestroySingleton()
    {
        if (instance != null)
            Destroy(instance);
    }

    protected virtual void OnEnable()
    {
       InitializeBehaviour();
    }

    protected virtual void OnDisable()
    {
        FinishBehaviour();
    }

    private void OnApplicationQuit()
    {
        FinishBehaviour();
    }
}