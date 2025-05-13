/// <summary>
/// 接收服务器消息体
/// </summary>
public class Accept<T> where T : class
{
    public int code;
    public string message;
    public T data;
}