using SISGED.Shared.DTOs;

namespace SISGED.Client.Helpers
{
    public class DateFilterItem
    {
        public DateFilterDTO DateFilter { get; set; }

        public string Name { get; set; }

        public DateFilterItem(DateFilterDTO dateFilter, string name)
        {
            DateFilter = dateFilter;
            Name = name;
        }
    }
}
