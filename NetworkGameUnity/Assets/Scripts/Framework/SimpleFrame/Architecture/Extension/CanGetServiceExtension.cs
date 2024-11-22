namespace SimpleFrame
{
    public static class CanGetServiceExtension
    {
        public static TService GetService<TService>(this ICanGetService self) where TService : IService
        {
            return mArchitecture.GetService<TService>();
        }

        private static IArchitecture mArchitecture;

        public static void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }
}