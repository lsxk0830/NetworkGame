public class ByteArray
{
    /// <summary>
    /// ������
    /// </summary>
    public byte[] bytes;

    /// <summary>
    /// �ɴӻ�������ȡ��λ��
    /// </summary>
    public int readIdx;

    /// <summary>
    /// �ɴӻ�����д��λ��
    /// </summary>
    public int writeIdx;

    /// <summary>
    /// ������ʣ�����ݳ���
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