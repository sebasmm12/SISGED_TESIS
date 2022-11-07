using SISGED.Shared.Models.Responses.PublicDeed;
using Entities = SISGED.Shared.Entities;

namespace SISGED.Client.Helpers
{
    public class Item
    {
        public string Name { get; set; } = default!;
        public object Value { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public string CurrentPlace { get; set; } = "tools";
        public string OriginPlace { get; set; } = default!;
        public Entities.Client Client { get; set; } = default!;
        public string ItemStatus { get; set; } = "none";
        public PublicDeedFilterResponse? PublicDeed { get; set; }

    }
}
