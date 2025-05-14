using Newtonsoft.Json;
using System.Net;
using System.Text;

/// <summary>
/// 异常处理中间件
/// </summary>
public static class ExceptionMiddleware
{
    public static async Task Handle(HttpListenerContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (ArgumentException ex)
        {
            await HandleAsync(context, 400, ex.Message); // 参数验证失败
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleAsync(context, 401, ex.Message); // 权限校验未通过
        }
        catch (Exception ex)
        {
            await HandleAsync(context, 500, ex.Message); // 未预料的服务端错误
        }
    }

    private static async Task HandleAsync(HttpListenerContext context, int stateCode, string msg)
    {
        var response = new { code = stateCode, message = msg };
        var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        context.Response.StatusCode = 500;
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
    }
}

/// <summary>
/// 【跨域支持】响应头自动添加、预检请求(OPTIONS)快速处理、全开放策略（生产环境建议细化）
/// </summary>
public static class CorsMiddleware
{
    public static async Task Handle(HttpListenerContext context, Func<Task> next)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 204;
            context.Response.Close();
            return;
        }

        await next();
    }
}