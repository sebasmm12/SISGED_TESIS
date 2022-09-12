using SISGED.Shared.Models.Queries.Dossier;

namespace SISGED.Shared.DTOs
{
    public class DossierFiltersConditionDTO<T>
    {
        public Predicate<DossierHistoryQuery> Condition { get; set; } = default!;
        public Func<T, DossierHistoryQuery, T> Result { get; set; } = default!;
    }
}
