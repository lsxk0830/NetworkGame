namespace SimpleFrame
{
    public static class CanGetUtilityExtension
    {
        public static TUtility GetUtility<TUtility>(this ICanGetUtility self) where TUtility : IUtility
        {
            return mArchitecture.GetUtility<TUtility>();
        }

        private static IArchitecture mArchitecture;

        public static void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }
}