namespace SISGED.Shared.Models.Responses.Tray
{
    public class UserTrayResponse
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string UserLastName { get; set; } = default!;
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            var other = obj as UserTrayResponse;

            return other?.UserId == UserId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public override string ToString()
        {
            return $"{UserName} {UserLastName}";
        }
    }
}
