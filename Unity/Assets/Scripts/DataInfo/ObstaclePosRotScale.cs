using System;

/// <summary>
/// 障碍物坐标、旋转、缩放
/// </summary>
[Serializable]
public struct ObstaclePosRotScale
{
    public float PosX;
    public float PosY;
    public float PosZ;

    public float RotX;
    public float RotY;
    public float RotZ;

    public float ScaleX;
    public float ScaleY;
    public float ScaleZ;
}