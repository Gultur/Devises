namespace LuccaDevises.Shared;


public class Result<T>
{
    public ResultState State { get; internal set; }

    public string Message { get; internal set; }

    // Il ne faudrait pas pouvoir acceder à la valeur en cas de Failure
    public T Value { get; internal set; }

    public bool IsFailure => this.State == ResultState.FAILURE;
    public bool IsSuccess => this.State == ResultState.SUCCESS;

    public static Result<T> Failure(string failureMessage)
    {
        return new Result<T>()
        {
            State = ResultState.FAILURE,
            Message = failureMessage,
            Value = default,
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
