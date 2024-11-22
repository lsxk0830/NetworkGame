namespace SimpleFrame
{
    public static class CanSendCommandExtension
    {
        public static void SendCommand<TCommand>(this ICanCommand self) where TCommand : ICommand, new()
        {
            mArchitecture.SendCommand<TCommand>();
        }

        public static void SendCommand<TCommand>(this ICanCommand self, TCommand command) where TCommand : ICommand
        {
            mArchitecture.SendCommand<TCommand>(command);
        }

        private static IArchitecture mArchitecture;

        public static void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }
}
