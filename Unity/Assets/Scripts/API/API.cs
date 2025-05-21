public static class API
{
#if UNITY_EDITOR
    public static string Login = "http://127.0.0.1:5000/api/login"; // 登录Post
#else
    public static string Login = "http://111.229.57.137:5000/api/login";
#endif

#if UNITY_EDITOR
    public static string Register = "http://127.0.0.1:5000/api/register"; // 注册Post
#else
    public static string Register = "http://111.229.57.137:5000/api/register";
#endif

#if UNITY_EDITOR
    public static string GetAvatar = "http://127.0.0.1:5000/api/getavatar"; // 获取用户头像信息
#else
    public static string GetAvatar = "http://111.229.57.137:5000/api/getavatar";
#endif

#if UNITY_EDITOR
    public static string GetUser = "http://127.0.0.1:5000/api/getuser"; // 获取用户信息Get
#else
    public static string GetUser = "http://111.229.57.137:5000/api/getuser";
#endif

#if UNITY_EDITOR
    public static string GetRooms = "http://127.0.0.1:5000/api/getrooms"; // 获取所有房间
#else
    public static string GetRooms = "http://111.229.57.137:5000/api/getrooms";
#endif
}