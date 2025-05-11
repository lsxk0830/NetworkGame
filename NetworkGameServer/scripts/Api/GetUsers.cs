using System.Net;

public partial class HTTPManager
{
    private void HandleAllUsers(HttpListenerContext context)
    {
        int page = int.Parse(context.Request.QueryString["page"] ?? "1");
        int pageSize = 10;

        var users = DbManager.Query<User>(
            "SELECT * FROM users ORDER BY id LIMIT @offset,@limit",
            new { offset = (page - 1) * pageSize, limit = pageSize }
        );

        SendResponse(context, 200, users);
    }
}