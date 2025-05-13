public static class API
{
#if UNITY_EDITOR
    public static string Login = "http://127.0.0.1:5000/api/login";
#else
    public static string Login = "http://111.229.57.137:5000/api/login";
#endif
}