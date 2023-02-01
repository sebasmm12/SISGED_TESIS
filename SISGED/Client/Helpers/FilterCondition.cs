namespace SISGED.Client.Helpers
{
    public class FilterCondition<TRequest, TResponse>
    {
        public Predicate<TRequest> Condition { get; set; } = default!;
        public Func<TResponse, TRequest, TResponse> Result { get; set; } = default!;
    }
}
