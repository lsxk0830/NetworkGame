using System;
using System.Threading;

namespace SimpleFrame
{
    public abstract class ThreadSingleton<T> where T : new()
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    object lockObject = _lock;
                    Monitor.Enter(lockObject);
                    try
                    {
                        if (_instance == null)
                            _instance = Activator.CreateInstance<T>();
                    }
                    finally
                    {
                        Monitor.Exit(lockObject);
                    }
                }
                return _instance;
            }
        }
    }
}