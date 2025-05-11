using System.Net;

public partial class HTTPManager
{
    private async void HandleAvatarUpload(HttpListenerContext context)
    {
        try
        {
            // 从请求头获取用户ID
            string userId = context.Request.Headers["X-User-Id"];

            // 解析multipart/form-data
            var fileData = await ParseMultipartForm(context.Request);

            // 保存文件到本地
            string fileName = $"{Guid.NewGuid()}.png";
            string savePath = Path.Combine("Avatars", fileName);
            await File.WriteAllBytesAsync(savePath, fileData);

            // 更新数据库[10](@ref)
            DbManager.ExecuteNonQuery(
                "UPDATE users SET avatar_path=@path WHERE id=@id",
                new { path = savePath, id = userId }
            );

            SendResponse(context, 200, new { Status = "Success", Path = savePath });
        }
        catch (Exception ex)
        {
            SendResponse(context, 400, new { Error = ex.Message });
        }
    }

    // Multipart解析辅助方法
    private async Task<byte[]> ParseMultipartForm(HttpListenerRequest request)
    {
        using var ms = new MemoryStream();
        await request.InputStream.CopyToAsync(ms);
        // 实际开发需完整解析boundary[9,11](@ref)
        return ms.ToArray();
    }
}