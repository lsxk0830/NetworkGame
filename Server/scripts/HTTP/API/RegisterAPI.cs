using System.Net;

public static partial class AuthController
{
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
        context.Response.OutputStream.Close();
    }
}