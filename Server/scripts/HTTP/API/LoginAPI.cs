using System.Net;

public static partial class AuthController
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
}