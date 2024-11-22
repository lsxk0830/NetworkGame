namespace SimpleFrame
{
    /// <summary>
    /// Controller层默认为显示层，即继承自MonoBehaviour的可视化层。
    /// 当然，根据项目的特殊性，有些项目场景中无脚本的挂载。
    /// 所以，操控Unity场景面板的脚本都可以默认为可视化层，即挂载IController接口
    /// </summary>
    public interface IController : ICanEvent, ICanCommand, ICanQuery, ICanGetUtility
    {
    }
}