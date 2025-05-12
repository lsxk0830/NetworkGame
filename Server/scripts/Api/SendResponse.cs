using System.Net;
using System.Text;
using System.Text.Json;

public partial class HTTPManager
{
    // 统一响应处理
    private void SendResponse(HttpListenerContext ctx, int statusCode, object data)
    {
        ctx.Response.StatusCode = statusCode;
        ctx.Response.ContentType = "application/json";

        string json = JsonSerializer.Serialize(data);
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        ctx.Response.Close();
    }
}