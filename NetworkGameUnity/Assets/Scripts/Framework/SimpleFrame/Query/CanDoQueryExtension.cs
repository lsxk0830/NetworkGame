namespace SimpleFrame
{
    public static class CanDoQueryExtension
    {
        public static Result DoQuery<TQuery, Result>(this ICanQuery self) where TQuery : IQuery<Result>, new()
        {
            return mArchitecture.DoQuery<TQuery, Result>();
        }

        public static Result DoQuery<TQuery, Result>(this ICanQuery self, IQuery<Result> query) where TQuery : IQuery<Result>, new()
        {
            return mArchitecture.DoQuery<TQuery, Result>(query);
        }

        private static IArchitecture mArchitecture;

        public static void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }
}