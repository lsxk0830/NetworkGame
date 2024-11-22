namespace SimpleFrame
{
    public abstract class AbsatractInject
    {
        public AbsatractInject()
        {
            DIContainer.InjectDependencies(this);
        }
    }
}