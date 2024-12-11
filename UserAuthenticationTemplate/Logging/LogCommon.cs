namespace UserAuthenticationTemplate.Logging
{
    public static partial class Log
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "The following required arguments are null: '{Arguments}'.")]
        private static partial void ArgumentNull(ILogger logger, string arguments);

        public static void LogArgumentNull(this ILogger logger, params string[] arguments)
        {
            ArgumentNull(logger, string.Join(", ", arguments.Select(a => a)));
        }
    }
}
