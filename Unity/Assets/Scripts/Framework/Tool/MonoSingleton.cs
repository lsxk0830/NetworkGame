using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void OnAwake() { }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this.GetComponent<T>();
            instance.OnAwake();
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }
}