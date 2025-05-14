using System.Net;

/// <summary>
/// 【服务入口】服务器生命周期管理、中间件和路由系统初始化、HTTP请求监听与分发
/// </summary>
public static class HTTPManager
{
    private static readonly RouteTable routeTable = new();
    private static readonly MiddlewarePipeline pipeline = new();

    public static void Initialize()
    {
        pipeline.Use(ExceptionMiddleware.Handle);
        pipeline.Use(CorsMiddleware.Handle);

        routeTable.RegisterControllers();
    }

    public static async Task StartAsync(CancellationToken token)
    {
        Initialize();

        using var listener = new HttpListener();
#if DEBUG
        listener.Prefixes.Add("http://127.0.0.1:5000/"); // "http://*:5000/"绑定到所有地址.*需要管理员权限
#else
        listener.Prefixes.Add("http://0.0.0.0:5000/");
#endif
        listener.Start();
        Console.WriteLine($"HTTP监听服务启动");

        var finalHandler = routeTable.HandleRequest;
        var pipelineHandler = pipeline.Build(finalHandler);

        while (!token.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => pipelineHandler(context));
        }
    }
}