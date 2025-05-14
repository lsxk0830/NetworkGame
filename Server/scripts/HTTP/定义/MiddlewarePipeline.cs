using System.Net;

/// <summary>
/// 【中间件管道】管理中间件执行顺序、构建中间件处理链、支持中间件的灵活扩展
/// </summary>
public class MiddlewarePipeline
{
    private readonly List<Func<HttpListenerContext, Func<Task>, Task>> _middlewares = new();

    public void Use(Func<HttpListenerContext, Func<Task>, Task> middleware)
    {
        _middlewares.Add(middleware);
    }

    public Func<HttpListenerContext, Task> Build(Func<HttpListenerContext, Task> finalHandler)
    {
        var handler = finalHandler;
        foreach (var middleware in _middlewares.AsEnumerable().Reverse()) // 反转序列中元素的顺序
        {
            var temp = handler;
            handler = ctx => middleware(ctx, () => temp(ctx));
        }
        return handler;
    }
}