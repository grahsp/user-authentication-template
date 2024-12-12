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

        [LoggerMessage(EventId = 1693, Level = LogLevel.Warning, Message = "Validation failed on object '{ObjectName}': {Errors}")]
        private static partial void ValidationFailed(ILogger logger, string objectName, string errors);

        public static void LogValidationFailed(this ILogger logger, string objectName, params string[] errors)
        {
            var errorString = string.Join(" | ", errors);
            ValidationFailed(logger, objectName, errorString);
        }
    }
}
