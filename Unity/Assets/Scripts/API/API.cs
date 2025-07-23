public static class API
{
#if DEBUG
    public static string IP = "127.0.0.1"; // Socket连接IP
#else
    public static string IP = "111.229.57.137";
#endif

#if DEBUG
    public static string Login = "http://127.0.0.1:5000/api/login"; // 登录Post
#else
    public static string Login = "http://111.229.57.137:5000/api/login";
#endif

#if DEBUG
    public static string Register = "http://127.0.0.1:5000/api/register"; // 注册Post
#else
    public static string Register = "http://111.229.57.137:5000/api/register";
#endif

#if DEBUG
    public static string GetAvatar = "http://127.0.0.1:5000/api/getavatar"; // 获取用户头像信息
#else
    public static string GetAvatar = "http://111.229.57.137:5000/api/getavatar";
#endif


#if DEBUG
    public static string UploadAvatar = "http://127.0.0.1:5000/api/UploadAvatar"; // 上传用户头像信息
#else
    public static string UploadAvatar = "http://111.229.57.137:5000/api/UploadAvatar";
#endif


#if DEBUG
    public static string GetUser = "http://127.0.0.1:5000/api/getuser"; // 获取用户信息Get
#else
    public static string GetUser = "http://111.229.57.137:5000/api/getuser";
#endif
}