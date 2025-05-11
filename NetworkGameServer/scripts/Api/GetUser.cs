using System.Net;

public partial class HTTPManager
{
    private void HandleSingleUser(HttpListenerContext context)
    {
        string userId = context.Request.Url.Segments.Last();

        var user = DbManager.QuerySingle<User>(
            "SELECT * FROM users WHERE id=@id",
            new { id = userId }
        );

        SendResponse(context, user != null ? 200 : 404, user);
    }
}