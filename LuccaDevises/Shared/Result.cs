namespace LuccaDevises.Shared
{
    public class Result
    {
        public ResultState State { get; private set; }

        public string? Message { get; private set; } 


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
