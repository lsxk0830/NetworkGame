using Newtonsoft.Json;
using System.Net;
using System.Text;

/// <summary>
/// 【业务控制器】处理具体业务逻辑、请求参数反序列化、业务数据验证、响应数据封装
/// </summary>
public static partial class AuthController
{
    private static async Task<T?> ReadBodyAsync<T>(this HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        string result = await reader.ReadToEndAsync();
        T? t = JsonConvert.DeserializeObject<T>(result);
        return t;
    }

    private static async Task SendResponse(HttpListenerContext context, int code, object data)
    {
        string jsonResponse = JsonConvert.SerializeObject(new { code, data });
        byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;

        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}