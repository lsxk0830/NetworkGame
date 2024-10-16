using proto.TestProto;
using UnityEngine;

public class ProtoTest : MonoBehaviour
{
    void Start()
    {
        TestProto testProto = new TestProto();
        testProto.x = 214;
        byte[] bs = Encode(testProto);
        Debug.Log($"{System.BitConverter.ToString(bs)}");
        Debug.Log($"获取协议名：{testProto.ToString()}");
        ProtoBuf.IExtensible m = Decode("proto.TestProto.TestProto", bs, 0, bs.Length);
        TestProto tp2 = (TestProto)m;
        Debug.Log($"{tp2.x}");
    }

    /// <summary>
    /// 将protobuf 对象序列化成Byte数组
    /// </summary>
    public static byte[] Encode(ProtoBuf.IExtensible msgBase)
    {
        using (var memory = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }

    /// <summary>
    /// 解码
    /// </summary>
    public static ProtoBuf.IExtensible Decode(string protoName, byte[] bytes, int offset, int count)
    {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count))
        {
            System.Type t = System.Type.GetType(protoName);
            return (ProtoBuf.IExtensible)ProtoBuf.Serializer.NonGeneric.Deserialize(t, memory);
        }
    }
}
