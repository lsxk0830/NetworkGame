namespace SimpleFrame
{
    public static class CanGetModelExtension
    {
        public static TModel GetModel<TModel>(this ICanGetModel self) where TModel : IModel
        {
            return mArchitecture.GetModel<TModel>();
        }

        private static IArchitecture mArchitecture;

        public static void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }
}