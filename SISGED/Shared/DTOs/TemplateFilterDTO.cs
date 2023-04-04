namespace SISGED.Shared.DTOs
{
    public class TemplateFilterDTO
    {
        public TemplateFilterDTO(string senderUserType, string receiverUserType, string actionId, string type)
        {
            SenderUserType = senderUserType;
            ReceiverUserType = receiverUserType;
            ActionId = actionId;
            Type = type;
        }

        public string SenderUserType { get; set; } = default!;
        public string ReceiverUserType { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
