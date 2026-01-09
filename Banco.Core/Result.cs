namespace Banco.Core
{
    public class Result
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public T Value { get; set; }

        private Result(bool success, string? error, T value)
        {
            Success = success;
            Error = error; 
            Value = value;
        }
        public static Result<T> Ok(T value)
        {
            return new Result<T>
            {
                Success = true,
                Value = value
            };
        }
        public static Result<T> Fail(string error)
        {
            return new Result<T>
            {
                Success = false,
                Error = error
            };
        }
    }
}