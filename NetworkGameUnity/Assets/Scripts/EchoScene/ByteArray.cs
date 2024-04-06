public class ByteArray
{
    /// <summary>
    /// 默认大小
    /// </summary>
    private const int DEFAULF_SIZE = 1024;

    /// <summary>
    /// 缓冲区
    /// </summary>
    public byte[] bytes;

    /// <summary>
    /// 缓冲区容量
    /// </summary>
    public int capacity;

    /// <summary>
    /// 初始长度
    /// </summary>
    public int initSize;

    /// <summary>
    /// 可从缓冲区读取的位置，缓冲区【有效数据】的起始位置 [0][3][c][a][t][0][2][h][i] readIdx = 5【可能为】
    /// </summary>
    public int readIdx;

    /// <summary>
    /// 可从缓冲区写的位置,缓冲区【有效数据】的末尾 [0][3][c][a][t][0][2][h][i] writeIdx = 9
    /// </summary>
    public int writeIdx;

    /// <summary>
    /// 缓冲区还可容纳的字节数
    /// </summary>
    public int remain
    {
        get
        {
            return capacity - writeIdx;
        }
    }

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
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public ByteArray(int size = DEFAULF_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }
}