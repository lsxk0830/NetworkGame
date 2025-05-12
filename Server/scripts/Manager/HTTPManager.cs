using System.Net;  // 包含HttpListenerContext类
using System;       // 包含Uri类（Url属性返回Uri对象）

public partial class HTTPManager
{
    private HttpListener _listener;

    private readonly Dictionary<(string Path, string Method), Action<HttpListenerContext>> _routes = new();

    public HTTPManager()
    {
        // 注册路由
        _routes.Add(("/api/avatar", "POST"), HandleAvatarUpload);
        _routes.Add(("/api/avatar", "GET"), HandleAvatarDownload);
        _routes.Add(("/api/users/{id}", "GET"), HandleSingleUser);
        _routes.Add(("/api/users", "GET"), HandleAllUsers);
    }

    public async Task StartAsync(string url = "http://localhost:9988/")
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
        _listener.Start();

        while (_listener.IsListening)
        {
            var context = await _listener.GetContextAsync();
            _ = Task.Run(() => ProcessRequest(context)); // 异步处理请求
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        try
        {
            var path = context.Request.Url.AbsolutePath;
            var method = context.Request.HttpMethod;

            // 动态匹配路由
            var handler = _routes.FirstOrDefault(x =>
                path.StartsWith(x.Key.Path.Replace("{id}", "")) &&
                method == x.Key.Method
            ).Value;

            handler?.Invoke(context);
        }
        catch (Exception ex)
        {
            SendResponse(context, 500, new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}