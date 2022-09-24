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

    public enum ResultState : short
    {
        SUCCESS,

        FAILURE,
    }
}
