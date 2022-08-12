using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.PublicDeed;
using SISGED.Shared.Models.Responses.PublicDeed;

namespace SISGED.Server.Services.Repositories
{
    public class PublicDeedsService : IPublicDeedsService
    {
        public async Task<IEnumerable<PublicDeed>> Filter(string term)
        {
            throw new NotImplementedException();
        }

        public async Task<PublicDeed> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PublicDeedFilterResponse>> SpecialFilter(PublicDeedSearchParametersFullFilterRequest searchparameters)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateResult> updatePublicDeedBySignatureConclusion(PublicDeed ep)
        {
            throw new NotImplementedException();
        }
    }
}
