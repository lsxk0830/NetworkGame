using System.Net;

public static partial class AuthController
{
    [Route("/api/uploadAvatar", "POST")]
    public static async Task UploadAvatar(HttpListenerContext context)
    {
        UploadAvatarData? request = await context.ReadBodyAsync<UploadAvatarData>();

        if (request == null || request.avatarBytes == null || request.avatarBytes.Length == 0)
        {
            await SendResponse(context, 400, "图片上传失败");
            return;
        }
        User? user = UserManager.GetUser(request.ID);
        if (user == null)
        {
            await SendResponse(context, 401, "用户不存在");
            return;
        }
        string avatarPath = request.ID + "_" + GetTimeStamp() + ".png";
        // 2. 拼接本地头像文件路径（假设头像存储在 ./avatars/ 目录下）
#if DEBUG
        string localPath = Path.Combine("E:\\Temp", avatarPath);
#else
        string localPath = Path.Combine("/www/wwwroot/TankServer/Tex", avatarPath);
#endif
        await File.WriteAllBytesAsync(localPath, request.avatarBytes);
        user.AvatarPath = avatarPath;
        bool result = DbManager.UpdateUser(user); //数据库更新

        if (result)
        {
            var obj = new { result = "更新用户头像成功" };
            Console.WriteLine($"更新用户头像成功");
            await SendResponse(context, 200, obj);
        }
        else
        {
            var obj = new { result = "更新用户头像失败" };
            Console.WriteLine($"更新用户头像失败");
            await SendResponse(context, 500, "服务器错误");
        }
    }
}