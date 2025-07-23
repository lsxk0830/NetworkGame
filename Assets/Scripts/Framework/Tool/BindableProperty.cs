using System;

/// <summary>
/// 绑定属性，监听数值变化
/// </summary>
/// <typeparam name="T">要监听变化的数</typeparam>
public class BindableProperty<T>
{
    private T mValue;
    public T Value
    {
        get => mValue;
        set
        {
            if (value.Equals(mValue)) return;
            mValue = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public Action<T> OnValueChanged = mValue => { };

    /// <summary>
    /// 初始化时赋值但不出发监听事件
    /// </summary>
    public BindableProperty(T defaultValue = default)
    {
        mValue = defaultValue;
    }
}
