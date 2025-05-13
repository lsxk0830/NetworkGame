using Newtonsoft.Json;
using System.Net;
using System.Text;

public class HTTPManager
{
    public static async Task StartAsync(CancellationToken token)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://*:5000/");
        listener.Start();

        try
        {
            Console.WriteLine($"HTTP 监听启动");
            var acceptContextTask = listener.GetContextAsync();
            while (!token.IsCancellationRequested)
            {
                var completedTask = await Task.WhenAny(
                    acceptContextTask,
                    Task.Delay(-1, token)
                );

                if (completedTask == acceptContextTask)
                {
                    var context = await acceptContextTask;
                    _ = ProcessRequestAsync(context, token);
                    acceptContextTask = listener.GetContextAsync(); // 重置任务
                }
            }
        }
        finally
        {
            listener.Close();
        }
    }

    static async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken token)
    {
        try
        {
            // 新增跨域头
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "POST");
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/api/login")
            {
                // 读取请求内容
                using var reader = new StreamReader(context.Request.InputStream);
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine($"Post接收：{requestBody}");
                byte[] buffer;

                if (string.IsNullOrEmpty(requestBody))
                {
                    await SendResponse(context, 400, "Bad Request", "用户名或密码不能为空");
                    return;
                }
                // 解析请求数据
                LoginRequest? request = JsonConvert.DeserializeObject<LoginRequest>(requestBody);
                if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.PW))
                {
                    await SendResponse(context, 400, "Bad Request", "用户名或密码不能为空");
                    return;
                }
                //模拟数据库查询
                User user = DbManager.Login(request.Name, request.PW);
                if (user == null)
                {
                    await SendResponse(context, 401, "Unauthorized", "用户名或密码错误");
                    return;
                }

                //登录成功，返回用户信息 + Token
                //user.token = GenerateJwtToken(user.Username) // 生成 JWT Token（示例）
                await SendResponse(context, 200, "OK", user);
            }
            else
            {
                context.Response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
                await context.Response.OutputStream.WriteAsync(buffer);
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            byte[] buffer = Encoding.UTF8.GetBytes($"Server Error: {ex.Message}");
            await context.Response.OutputStream.WriteAsync(buffer);
        }
        finally
        {
            context.Response.Close();
        }
    }

    /// <summary>
    /// 辅助方法：发送标准化的 JSON 响应
    /// </summary>
    private static async Task SendResponse(HttpListenerContext context, int statusCode, string message, object data)
    {
        var response = new
        {
            code = statusCode,
            message = message,
            data = data
        };

        string jsonResponse = JsonConvert.SerializeObject(response);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;

        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }
}