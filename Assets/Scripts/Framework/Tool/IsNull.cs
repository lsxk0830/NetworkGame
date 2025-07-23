using UnityEngine;

public static partial class ToolExtension
{
    public static bool IsNull(this GameObject go)
    {
        return ReferenceEquals(go, null);
    }

    public static bool InstanceIsNull(this object obj)
    {
        return ReferenceEquals(obj, null);
    }
}