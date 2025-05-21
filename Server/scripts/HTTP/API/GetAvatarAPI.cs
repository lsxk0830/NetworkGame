using System.Net;

public static partial class AuthController
{
    [Route("/api/getavatar", "GET")]
    public static async Task GetAvatar(HttpListenerContext context)
    {
        try
        {
            // 1. 从查询参数获取头像路径
            string? avatarPath = context.Request.QueryString["path"];
            if (string.IsNullOrEmpty(avatarPath))
            {
                await SendResponse(context, 400, "缺少 path 参数");
                return;
            }

            // 2. 拼接本地头像文件路径（假设头像存储在 ./avatars/ 目录下）
#if DEBUG
            string localPath = Path.Combine("E:\\Temp", avatarPath);
#else
            string localPath = Path.Combine("E:\\Temp", avatarPath);
#endif
            Console.WriteLine($"头像路径:{localPath}");
            if (!File.Exists(localPath))
            {
                await SendResponse(context, 404, "头像文件不存在");
                return;
            }

            // 3. 读取文件并返回（直接传输二进制数据）
            byte[] fileBytes = await File.ReadAllBytesAsync(localPath);

            context.Response.StatusCode = 200;
            context.Response.ContentType = GetContentType(localPath); // 根据扩展名设置 Content-Type
            context.Response.ContentLength64 = fileBytes.Length;

            await context.Response.OutputStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            context.Response.OutputStream.Close();
        }
        catch (Exception ex)
        {
            await SendResponse(context, 500, $"服务器错误: {ex.Message}");
        }
    }

    // 根据文件扩展名返回对应的 Content-Type
    private static string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => "application/octet-stream" // 默认二进制流
        };
    }
}