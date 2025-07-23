public class Singleton<T> where T : class, new()
{
    public Singleton() { }

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();
            return instance;
        }
    }
}
