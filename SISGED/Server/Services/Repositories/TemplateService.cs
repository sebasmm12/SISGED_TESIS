using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class TemplateService : ITemplateService
    {
        private readonly IMongoCollection<Template> _templatesCollection;

        public string CollectionName => "plantillas";

        public TemplateService(IMongoDatabase mongoDatabase)
        {
            _templatesCollection = mongoDatabase.GetCollection<Template>(CollectionName);
        }

        public async Task<Template> GetTemplateAsync(TemplateFilterDTO templateFilterDTO)
        {
            var template = await _templatesCollection
                                    .Find(template => template.SenderUserType == templateFilterDTO.SenderUserType
                                            && template.ReceiverUserType == templateFilterDTO.ReceiverUserType
                                            && template.ActionId == templateFilterDTO.ActionId
                                            && template.Type == templateFilterDTO.Type)
                                    .FirstOrDefaultAsync();

            if (template is null) throw new Exception($"No se pudo encontrar la plantilla del tipo de usuario { templateFilterDTO.SenderUserType } como emisor y { templateFilterDTO.ReceiverUserType } como receptor");

            return template;
        }
    }
}
