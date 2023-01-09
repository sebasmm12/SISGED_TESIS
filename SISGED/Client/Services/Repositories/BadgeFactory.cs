using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class BadgeFactory : IBadgeFactory
    {
        private readonly IEnumerable<BadgePreview> Badges = new List<BadgePreview>()
        {
            new(Color.Transparent, Icons.Material.Filled.Circle, false),
            new(Color.Success, Icons.Material.Filled.Check, true)
        };

        public BadgePreview GetBadgePreview(bool selected)
        {
            var badgePreview = Badges.FirstOrDefault(badge => badge.Selected == selected);

            if (badgePreview is null) throw new Exception($"No se pudo encontrar la previsualización del badge");

            return badgePreview;
        }
    }
}


