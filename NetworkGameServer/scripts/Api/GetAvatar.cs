using System.Net;

public partial class HTTPManager
{
    private void HandleAvatarDownload(HttpListenerContext context)
    {
        string userId = context.Request.QueryString["userId"];

        var user = DbManager.QuerySingle<User>(
            "SELECT avatar_path FROM users WHERE id=@id",
            new { id = userId }
        );

        if (user?.AvatarPath != null)
        {
            byte[] imageBytes = File.ReadAllBytes(user.AvatarPath);
            context.Response.ContentType = "image/png";
            context.Response.OutputStream.Write(imageBytes, 0, imageBytes.Length);
        }
        else
        {
            SendResponse(context, 404, "Avatar not found");
        }
    }
}