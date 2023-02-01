namespace SISGED.Shared.DTOs
{
    public class FilterConditionDTO<TRequest, TResponse>
    {
        public Predicate<TRequest> Condition { get; set; } = default!;
        public Func<TResponse, TRequest, TResponse> Result { get; set; } = default!;
    }
}
