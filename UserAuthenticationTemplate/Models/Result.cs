﻿namespace UserAuthenticationTemplate.Models
{
    public abstract class ResultBase
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure { get => !IsSuccess; }
        public string[] Errors { get; private set; }
        public bool HasErrors { get => Errors.Length > 0; }

        public ResultBase(bool success, params string[] errors)
        {
            IsSuccess = success;
            Errors = [.. errors];
        }

        public static implicit operator bool(ResultBase result) => result.IsSuccess;
    }

    public class Result : ResultBase
    {
        public Result(bool success, params string[] errors) : base(success, errors)
        {
        }

        public static Result Success()
        {
            return new(true);
        }

        public static Result Failure(params string[] errors)
        {
            return new(false, errors);
        }

        public static bool Merge(params bool[] results)
        {
            return !results.Any(r => r == false);
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Success"
                : $"Failure: {string.Join(", ", Errors)}";
        }
    }

    public class Result<T> : ResultBase
    {
        private T? _data;
        public T Data { get => IsSuccess && _data != null ? _data : throw new ArgumentNullException(nameof(Data), "Cannot access data if result is failed"); }

        public Result(bool success, T? data = default, params string[] errors) : base(success, errors)
        {
            if (success && data == null)
                throw new ArgumentNullException(nameof(data), "Result cannot be set to success with null data!");

            _data = data;
        }

        public static Result<T> Success(T data)
        {
            return new(true, data);
        }

        public static Result<T> Failure(params string[] errors)
        {
            return new(false, default, errors);
        }

        public static Result<T> FromData(T? data, string failureMessage = "Data must not be null")
        {
            return data != null
            ? Success(data)
            : Failure(failureMessage);
        }

        public static implicit operator Result<T>(T data) => Success(data);

        public override string ToString()
        {
            return IsSuccess
                ? $"Success: {Data}"
                : $"Failure: {string.Join(", ", Errors)}";
        }
    }
}