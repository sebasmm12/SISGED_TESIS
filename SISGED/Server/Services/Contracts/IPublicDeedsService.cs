using MongoDB.Driver;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.PublicDeed;
using SISGED.Shared.Models.Responses.PublicDeed;

namespace SISGED.Server.Services.Contracts
{
    public interface IPublicDeedsService
    {
        public Task<IEnumerable<PublicDeed>> Filter(string term);
        public Task<UpdateResult> updatePublicDeedBySignatureConclusion(PublicDeed ep);
        public Task<IEnumerable<PublicDeedFilterResponse>> SpecialFilter(PublicDeedSearchParametersFullFilterRequest searchparameters);
        public Task<PublicDeed> GetById(string id);
    }
}
