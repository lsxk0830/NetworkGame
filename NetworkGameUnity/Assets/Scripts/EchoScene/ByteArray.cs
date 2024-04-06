public class ByteArray
{
    /// <summary>
    /// 缓冲区
    /// </summary>
    public byte[] bytes;

    /// <summary>
    /// 可从缓冲区读取的位置
    /// </summary>
    public int readIdx;

    /// <summary>
    /// 可从缓冲区写的位置
    /// </summary>
    public int writeIdx;

    /// <summary>
    /// 缓冲区剩余数据长度
    /// </summary>
    public int length
    {
        get
        {
            return writeIdx - readIdx;
        }
    }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public ByteArray(int size = -1)
    {
        if (size == -1)
            bytes = new byte[1024];
        else
            bytes = new byte[size];
    }
}