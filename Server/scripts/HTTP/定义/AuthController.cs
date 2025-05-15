using Newtonsoft.Json;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// 【业务控制器】处理具体业务逻辑、请求参数反序列化、业务数据验证、响应数据封装
/// </summary>
public static class AuthController
{
    [Route("/api/login", "POST")]
    public static async Task Login(HttpListenerContext context)
    {
        LoginRegisterRequest? request = await context.ReadBodyAsync<LoginRegisterRequest>();

        if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.PW))
        {
            await SendResponse(context, 400, "用户名或密码错误");
            return;
        }
        //模拟数据库查询
        User user = DbManager.Login(request.Name, request.PW);
        if (user == null)
        {
            await SendResponse(context, 401, "用户名或密码错误");
            return;
        }

        if (!UserManager.IsOnline(user.ID))
        {
            UserManager.AddUser(user.ID, user);
            Console.WriteLine($"用户登录成功");
        }
        else
        {
            Console.WriteLine($"用户已在线,旧用户踢下线");
            MsgKick msg = new MsgKick();
            UserManager.Send(user.ID, msg);
            UserManager.RemoveUser(user.ID);
        }
        await SendResponse(context, 200, user);
    }

    [Route("/api/register", "POST")]
    public static async Task Register(HttpListenerContext context)
    {
        LoginRegisterRequest? request = await context.ReadBodyAsync<LoginRegisterRequest>();

        if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.PW))
        {
            await SendResponse(context, 400, "用户名或密码不能为空");
            return;
        }
        //模拟数据库查询
        long id = DbManager.Register(request.Name, request.PW);
        if (id == -1)
        {
            await SendResponse(context, 401, "注册失败");
            return;
        }

        //登录成功，返回用户信息 + Token
        //user.token = GenerateJwtToken(user.Username) // 生成 JWT Token（示例）
        Console.WriteLine($"用户注册成功");
        await SendResponse(context, 200, id);
    }

    private static async Task<T?> ReadBodyAsync<T>(this HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        string result = await reader.ReadToEndAsync();
        T? t = JsonConvert.DeserializeObject<T>(result);
        return t;
    }

    private static async Task SendResponse(HttpListenerContext context, int stateCode, object sendData)
    {
        var response = new
        {
            code = stateCode,
            data = sendData
        };

        string jsonResponse = JsonConvert.SerializeObject(response);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

        context.Response.StatusCode = stateCode;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;

        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }
}