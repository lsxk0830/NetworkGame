using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;

/// <summary>
/// 【路由中枢】维护路由-处理方法的映射表、自动发现标注了[Route]的方法、请求路由匹配与分发
/// </summary>
public class RouteTable
{
    private readonly Dictionary<(string, string), Func<HttpListenerContext, Task>> routes = new();

    public void RegisterControllers()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) // GetExecutingAssembly:获取包含当前正在执行的代码的程序集
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                if (routeAttr != null)
                {
                    AddRoute
                    (
                        routeAttr.Path,
                        routeAttr.Method,
                        (Func<HttpListenerContext, Task>)Delegate.CreateDelegate // 创建Func<HttpListenerContext, Task>委托实例并将method转化成这个委托实例
                        (
                            typeof(Func<HttpListenerContext, Task>), method
                        )
                    );
                }
            }
        }
    }

    public void AddRoute(string path, string method, Func<HttpListenerContext, Task> handler)
    {
        routes[(path.ToLower(), method.ToUpper())] = handler;
    }

    public async Task HandleRequest(HttpListenerContext context)
    {
        //Console.WriteLine($"HandleRequest");
        var path = context.Request.Url?.AbsolutePath.ToLower() ?? "";
        var method = context.Request.HttpMethod.ToUpper();

        if (routes.TryGetValue((path, method), out var handler))
        {
            await handler(context);
        }
        else
        {
            await SendErrorResponse(context, 404, "Route not found");
        }
    }

    private static async Task SendErrorResponse(HttpListenerContext context, int code, string message)
    {
        var response = new { code, message };
        var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.Close();
    }
}