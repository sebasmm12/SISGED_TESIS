using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IBadgeFactory
    {
        BadgePreview GetBadgePreview(bool selected);
    }
}
