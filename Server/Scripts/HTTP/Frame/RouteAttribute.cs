/// <summary>
/// 路由属性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public string Path { get; }
    public string Method { get; }

    public RouteAttribute(string path, string method = "GET")
    {
        Path = path;
        Method = method;
    }
}