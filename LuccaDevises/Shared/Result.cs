namespace LuccaDevises.Shared
{
    public class Result
    {
        public ResultState State { get; internal set; }

        public string? Message { get; internal set; } 


        public static Result Failure(string failureMessage)
        {
            return new Result()
            {
                State = ResultState.FAILURE,
                Message = failureMessage,
            };
        }
        
        public static Result Success()
        {
            return new Result()
            {
                State = ResultState.SUCCESS,
            };
        }

        public bool IsFailure => this.State == ResultState.FAILURE;
        public bool IsSuccess => this.State == ResultState.SUCCESS;

    }

    public class Result<T> : Result
    {
        // Il ne faudrait pas pouvoir acceder à la valeur en cas de Failure
        public T Value { get; internal set; }

        public new static Result<T> Failure(string failureMessage)
        {
            return new Result<T>()
            {
                State = ResultState.FAILURE,
                Message = failureMessage,
                Value = default(T),
            };
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>()
            {
                State = ResultState.SUCCESS,
                Value = value,
            };
        }
    }

    public enum ResultState : short
    {
        SUCCESS,

        FAILURE,
    }
}
