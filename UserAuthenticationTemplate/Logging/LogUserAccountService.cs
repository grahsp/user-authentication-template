using UserAuthenticationTemplate.Services;

namespace UserAuthenticationTemplate
{
    public static partial class Log
    {
        #region Create
        [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "Successfully created user '{UserIdentifier}'.")]
        private static partial void CreateUserSuccess(ILogger logger, string userIdentifier);

        [LoggerMessage(EventId = 102, Level = LogLevel.Warning, Message = "Failed to create user '{UserIdentifier}'. {Errors}.")]
        private static partial void CreateUserFailure(ILogger logger, string userIdentifier, string errors);

        /// <summary>
        /// Logs the result of an attempt to create a user.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="success">Indicates whether the user creation was successful.</param>
        /// <param name="userIdentifier">A unique identifier for the user, such as a username or email.</param>
        /// <param name="errors">
        /// A string containing error messages if the user creation failed. This parameter is optional and defaults to an empty string.
        /// </param>
        /// <remarks>
        /// This method simplifies logging by wrapping the <c>CreateUserSuccess</c> and <c>CreateUserFailure</c> log methods.
        /// Ensure the <paramref name="errors"/> parameter is provided when <paramref name="success"/> is <c>false</c>.
        /// </remarks>
        public static void LogCreateUserResult(this ILogger<UserAccountService> logger, bool success, string userIdentifier, string errors = "")
        {
            if (success)
                CreateUserSuccess(logger, userIdentifier);
            else
                CreateUserFailure(logger, userIdentifier, errors);
        }
        #endregion

        #region FindByEmail
        [LoggerMessage(EventId = 103, Level = LogLevel.Information, Message = "User with email '{Email}' was found.")]
        private static partial void FindByEmailSuccess(ILogger logger, string email);

        [LoggerMessage(EventId = 104, Level = LogLevel.Warning, Message = "No user found with email '{Email}'. {Errors}")]
        private static partial void FindByEmailFailure(ILogger logger, string email, string errors);

        /// <summary>
        /// Logs the result of an attempt to find a user by their email address.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="success">Indicates whether the user was found.</param>
        /// <param name="email">The email address used to search for the user.</param>
        /// <param name="errors">
        /// A string containing error messages if the user was not found. This parameter is optional and defaults to an empty string.
        /// </param>
        /// <remarks>
        /// This method simplifies logging by wrapping the <c>FindByEmailSuccess</c> and <c>FindByEmailFailure</c> log methods.
        /// Ensure the <paramref name="errors"/> parameter is provided when <paramref name="success"/> is <c>false</c>.
        /// </remarks>
        public static void LogFindByEmailResult(this ILogger<UserAccountService> logger, bool success, string email, string errors = "")
        {
            if (success)
                FindByEmailSuccess(logger, email);
            else
                FindByEmailFailure(logger, email, errors);
        }
        #endregion

        #region FindByName
        [LoggerMessage(EventId = 105, Level = LogLevel.Information, Message = "User with username '{Username}' was found.")]
        private static partial void FindByNameSuccess(ILogger logger, string username);

        [LoggerMessage(EventId = 106, Level = LogLevel.Warning, Message = "No user found with username '{Username}'. {Errors}")]
        private static partial void FindByNameFailure(ILogger logger, string username, string errors);

        /// <summary>
        /// Logs the result of an attempt to find a user by their username.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="success">Indicates whether the user was found.</param>
        /// <param name="username">The username used to search for the user.</param>
        /// <param name="errors">
        /// A string containing error messages if the user was not found. This parameter is optional and defaults to an empty string.
        /// </param>
        /// <remarks>
        /// This method simplifies logging by wrapping the <c>FindByNameSuccess</c> and <c>FindByNameFailure</c> log methods.
        /// Ensure the <paramref name="errors"/> parameter is provided when <paramref name="success"/> is <c>false</c>.
        /// </remarks>
        public static void LogFindByNameResult(this ILogger<UserAccountService> logger, bool success, string username, string errors = "")
        {
            if (success)
                FindByNameSuccess(logger, username);
            else
                FindByNameFailure(logger, username, errors);
        }
        #endregion

        #region CheckPassword
        [LoggerMessage(EventId = 107, Level = LogLevel.Information, Message = "User '{UserIdentifier}' provided the correct password.")]
        private static partial void CheckPasswordSuccess(ILogger logger, string userIdentifier);

        [LoggerMessage(EventId = 108, Level = LogLevel.Warning, Message = "Incorrect password provided for user '{UserIdentifier}'. {Errors}")]
        private static partial void CheckPasswordFailure(ILogger logger, string userIdentifier, string errors);

        /// <summary>
        /// Logs the result of a password verification attempt for a user.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="success">Indicates whether the password was correct.</param>
        /// <param name="userIdentifier">A unique identifier for the user, such as a username or email.</param>
        /// <param name="errors">
        /// A string containing error messages if the password verification failed. This parameter is optional and defaults to an empty string.
        /// </param>
        /// <remarks>
        /// This method abstracts the logging of password verification results by using the <c>CheckPasswordSuccess</c> and <c>CheckPasswordFailure</c> log methods.
        /// Ensure the <paramref name="errors"/> parameter is provided when <paramref name="success"/> is <c>false</c>.
        /// </remarks>
        public static void LogChecKPasswordResult(this ILogger<UserAccountService> logger, bool success, string userIdentifier, string errors = "")
        {
            if (success)
                CheckPasswordSuccess(logger, userIdentifier);
            else
                CheckPasswordFailure(logger, userIdentifier, errors);
        }
        #endregion

        #region IsLockedOut
        [LoggerMessage(EventId = 109, Level = LogLevel.Information, Message = "User '{UserIdentifier}' is not locked out.")]
        private static partial void IsLockedOutSuccess(ILogger logger, string userIdentifier);

        [LoggerMessage(EventId = 110, Level = LogLevel.Warning, Message = "User '{UserIdentifier}' is locked out. {Errors}")]
        private static partial void IsLockedOutFailure(ILogger logger, string userIdentifier, string errors);

        /// <summary>
        /// Logs the result of a lockout status check for a user.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="isUserLockedOut">Indicates whether the user is not locked out.</param>
        /// <param name="userIdentifier">A unique identifier for the user, such as a username or email.</param>
        /// <param name="errors">
        /// A string containing error messages if the lockout status could not be verified. This parameter is optional and defaults to an empty string.
        /// </param>
        /// <remarks>
        /// This method provides a simplified way to log the lockout status of a user by using the <c>IsLockedOutSuccess</c> and <c>IsLockedOutFailure</c> log methods.
        /// Ensure the <paramref name="errors"/> parameter is provided when <paramref name="isUserLockedOut"/> is <c>false</c>.
        /// </remarks>
        public static void LogIsLockedOutResult(this ILogger<UserAccountService> logger, bool isUserLockedOut, string userIdentifier, string errors = "")
        {
            if (!isUserLockedOut)
                IsLockedOutSuccess(logger, userIdentifier);
            else
                IsLockedOutFailure(logger, userIdentifier, errors);
        }
        #endregion

        #region AccessFailedCount
        [LoggerMessage(EventId = 111, Level = LogLevel.Information, Message = "User '{UserIdentifier}' failed access count has increased.")]
        private static partial void AccessFailedCountSuccess(ILogger logger, string userIdentifier);

        [LoggerMessage(EventId = 112, Level = LogLevel.Warning, Message = "An error occurred increasing user '{UserIdentifier}' failed access count. {Errors}")]
        private static partial void AccessFailedCountFailure(ILogger logger, string userIdentifier, string errors);

        /// <summary>
        /// Logs the result of attempting to retrieve the failed login count for a user.
        /// </summary>
        /// <param name="logger">The logger instance associated with <see cref="UserAccountService"/>.</param>
        /// <param name="success">Indicates whether retrieving the login count was successful.</param>
        /// <param name="userIdentifier">A unique identifier for the user, such as a username or email.</param>
        /// <param name="errors">A string containing error messages if the operation fails. This is optional and defaults to an empty string.</param>
        /// <remarks>
        /// Use this method to log the success or failure of retrieving a user's login failure count. If the operation fails, provide a description of the error via the <paramref name="errors"/> parameter.
        /// </remarks>
        public static void LogAccessFailedCountResult(this ILogger<UserAccountService> logger, bool success, string userIdentifier, string errors = "")
        {
            if (success)
                AccessFailedCountSuccess(logger, userIdentifier);
            else
                AccessFailedCountFailure(logger, userIdentifier, errors);
        }
        #endregion
    }
}
