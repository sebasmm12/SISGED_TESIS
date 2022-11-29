namespace SISGED.Client.Helpers
{
    public class SolicitorFilterCondition<T>
    {
        public Predicate<SolicitorFilter> Condition { get; set; } = default!;
        public Func<T, SolicitorFilter, T> Result { get; set; } = default!;
    }
}
