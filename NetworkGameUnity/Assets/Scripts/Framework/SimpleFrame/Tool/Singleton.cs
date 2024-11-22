namespace SimpleFrame
{
    public class Singleton<T> where T : class, new()
    {
        private Singleton() { }

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
}