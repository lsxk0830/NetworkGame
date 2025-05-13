using System.Net;
using System.Text;

public class HTTPManager
{
    public static async Task StartAsync(CancellationToken token)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://*:5000/");
        listener.Start();

        try
        {
            Console.WriteLine($"HTTP 监听启动");
            var acceptContextTask = listener.GetContextAsync();
            while (!token.IsCancellationRequested)
            {
                var completedTask = await Task.WhenAny(
                    acceptContextTask,
                    Task.Delay(-1, token)
                );

                if (completedTask == acceptContextTask)
                {
                    var context = await acceptContextTask;
                    _ = ProcessRequestAsync(context, token);
                    acceptContextTask = listener.GetContextAsync(); // 重置任务
                }
            }
        }
        finally
        {
            listener.Close();
        }
    }

    static async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken token)
    {
        try
        {
            // 新增跨域头
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "POST");
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            if (context.Request.HttpMethod == "POST" &&
                context.Request.Url.AbsolutePath == "/api/login")
            {
                // 读取请求内容
                using var reader = new StreamReader(context.Request.InputStream);
                string requestBody = await reader.ReadToEndAsync();

                Console.WriteLine($"Post接收：{requestBody}");

                // 构造响应
                byte[] buffer = Encoding.UTF8.GetBytes($"Received: {requestBody}");

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer);
            }
            else
            {
                context.Response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
                await context.Response.OutputStream.WriteAsync(buffer);
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            byte[] buffer = Encoding.UTF8.GetBytes($"Server Error: {ex.Message}");
            await context.Response.OutputStream.WriteAsync(buffer);
        }
        finally
        {
            context.Response.Close();
        }
    }
}