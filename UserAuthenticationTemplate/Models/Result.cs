namespace UserAuthenticationTemplate.Models
{
    /// <summary>
    /// Represents the base class for operation results, providing common properties and functionality.
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure { get => !IsSuccess; }

        /// <summary>
        /// Gets an array of error messages associated with the result.
        /// </summary>
        public string[] Errors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there are any error messages.
        /// </summary>
        public bool HasErrors { get => Errors.Length > 0; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase"/> class.
        /// </summary>
        /// /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="errors">A list of error messages if the operation failed.</param>
        public ResultBase(bool success, params string[] errors)
        {
            IsSuccess = success;
            Errors = [.. errors];
        }

        /// <summary>
        /// Implicitly converts a <see cref="ResultBase"/> to a <see cref="bool"/>,
        /// returning <c>true</c> if the result is successful, or <c>false</c> otherwise.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        public static implicit operator bool(ResultBase result) => result.IsSuccess;
    }

    /// <summary>
    /// Represents the result of an operation, indicating success or failure.
    /// </summary>
    public class Result : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="errors">A list of error messages if the operation failed.</param>
        public Result(bool success, params string[] errors) : base(success, errors)
        {
        }

        /// <summary>
        /// Creates a successful <see cref="Result"/>.
        /// </summary>
        /// <returns>A successful <see cref="Result"/> instance.</returns>
        public static Result Success()
        {
            return new(true);
        }

        /// <summary>
        /// Creates a failed <see cref="Result"/> with the specified error messages.
        /// </summary>
        /// <param name="errors">A list of error messages describing the failure.</param>
        /// <returns>A failed <see cref="Result"/> instance.</returns>
        public static Result Failure(params string[] errors)
        {
            return new(false, errors);
        }

        /// <summary>
        /// Combines multiple boolean or results into a single result.
        /// </summary>
        /// <param name="results">An array of boolean values representing operation results.</param>
        /// <returns>
        /// <c>true</c> if all results are successful; otherwise, <c>false</c>.
        /// </returns>
        public static bool Merge(params bool[] results)
        {
            return !results.Any(r => r == false);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Result"/>.
        /// </summary>
        /// <returns>
        /// A string indicating whether the result is successful or failed,
        /// and any associated error messages if failed.
        /// </returns>
        public override string ToString()
        {
            return IsSuccess
                ? $"Success"
                : $"Failure: {string.Join(", ", Errors)}";
        }
    }

    /// <summary>
    /// Represents the result of an operation that returns a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the result.</typeparam>
    public class Result<T> : ResultBase
    {
        private T? _data;
        /// <summary>
        /// Gets the data associated with the result if the operation was successful.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the operation failed and data is accessed.</exception>
        public T Data { get => IsSuccess && _data != null ? _data : throw new ArgumentNullException(nameof(Data), "Cannot access data if result is failed"); }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="data">The data associated with the result if the operation was successful.</param>
        /// <param name="errors">A list of error messages if the operation failed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="success"/> is true and <paramref name="data"/> is null.</exception>
        public Result(bool success, T? data = default, params string[] errors) : base(success, errors)
        {
            if (success && data == null)
                throw new ArgumentNullException(nameof(data), "Result cannot be set to success with null data!");

            _data = data;
        }

        /// <summary>
        /// Creates a successful result containing the specified data.
        /// </summary>
        /// <param name="data">The data to associate with the successful result.</param>
        /// <returns>A <see cref="Result{T}"/> indicating success.</returns>
        public static Result<T> Success(T data)
        {
            return new(true, data);
        }

        /// <summary>
        /// Creates a failed result with the specified error messages.
        /// </summary>
        /// <param name="errors">The error messages associated with the failure.</param>
        /// <returns>A <see cref="Result{T}"/> indicating failure.</returns>
        public static Result<T> Failure(params string[] errors)
        {
            return new(false, default, errors);
        }

        /// <summary>
        /// Creates a result based on whether the specified data is null.
        /// </summary>
        /// <param name="data">The data to evaluate.</param>
        /// <param name="failureMessage">The error message to use if <paramref name="data"/> is null.</param>
        /// <returns>
        /// A successful <see cref="Result{T}"/> if <paramref name="data"/> is not null; otherwise, a failed result.
        /// </returns>
        public static Result<T> FromData(T? data, string failureMessage = "Data must not be null")
        {
            return data != null
            ? Success(data)
            : Failure(failureMessage);
        }

        /// <summary>
        /// Used to convert Result<T> incase you don't want to create a new Result and don't need the Data property.
        /// </summary>
        /// <returns>
        /// A <see cref="Result"/> with the same properties of <see cref="Result{T}"/> besides data. 
        /// </returns>
        public Result ToBase()
            => new(IsSuccess, Errors);

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="T"/> into a successful <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="data">The data to associate with the successful result.</param>
        public static implicit operator Result<T>(T data) => Success(data);

        /// <summary>
        /// Returns a string representation of the result, indicating success or failure.
        /// </summary>
        /// <returns>
        /// A string indicating success and the associated data, or failure with error messages.
        /// </returns>
        public override string ToString()
        {
            return IsSuccess
                ? $"Success: {Data}"
                : $"Failure: {string.Join(", ", Errors)}";
        }
    }
}