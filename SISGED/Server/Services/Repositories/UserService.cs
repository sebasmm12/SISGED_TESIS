using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Server.Services.Repositories
{
    public class UserService: IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Role> _rolesCollection;
        public string CollectionName => "usuarios";
        public UserService(IMongoDatabase mongoDatabase)
        {
            _usersCollection = mongoDatabase.GetCollection<User>(CollectionName);
            _rolesCollection = mongoDatabase.GetCollection<Role>("roles");
        }
        
        public async Task<List<User>> GetUsersAsync()
        {
            
            var users = await _usersCollection.Find(user => true).ToListAsync();

            if (users is null) throw new Exception("No se ha podido encontrar los usuarios registrados");

            return users;
        }

        public async Task CreateUserAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);

            if (string.IsNullOrEmpty(user.Id)) throw new Exception("No se pudo registrar el usuario al sistema");
            
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _usersCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();

            if (user is null) throw new Exception($"No se pudo encontrar el usuario solicitado con identificador {userId}");

            return user;
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            var user = await _usersCollection.Find(user => user.UserName == userName).FirstOrDefaultAsync();

            if (user is null) throw new Exception($"No se pudo encontrar el usuario solicitado con el nombre {userName}");

            return user;
        }

        public async Task<bool> VerifyUserExistsAsync(string userId)
        {
            var user = await _usersCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();

            return user is not null;
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, user.Id);
            var update = SetUserInformation(user);

            var updatedUser = await _usersCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedUser is null) throw new Exception("No se pudo actualizar el usuario");
        }

        private static UpdateDefinition<User> SetUserInformation(User user)
        {
            return Builders<User>.Update.Set("usuario", user.UserName)
                                        .Set("clave", user.Password)
                                        .Set("datos", user.Data)
                                        .Set("rol", user.Rol);
        }

        public async Task UpdateUserStateAsync(string userId, string state)
        {
            var userState = state == "activo" ? "inactivo" : "activo";

            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            var update = Builders<User>.Update.Set("estado", userState);

            var updatedUser = await _usersCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedUser is null) throw new Exception("No se pudo actualizar el usuario");
        }

        public async Task UpdateUserPasswordAsync(string userId, string password)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            var update = Builders<User>.Update.Set("clave", password);

            var updatedUser = await _usersCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedUser is null) throw new Exception("No se pudo actualizar el usuario");
        }

        public async Task<IEnumerable<User>> GetUsersByStateAsync(string userState)
        {
            var users = await _usersCollection.Find(user => user.State == userState).ToListAsync();

            if (users is null) throw new Exception($"No se ha podido encontrar los usuarios registrados con estado {userState}");

            return users;
        }

        public async Task<IEnumerable<ProsecutorUserInfoResponse>> GetProsecutorUsersAsync()
        {
            var prosecutorUsers = await _usersCollection.Aggregate<ProsecutorUserInfoResponse>(GetPersecutorsUserPipeline())
                                                        .ToListAsync();

            return prosecutorUsers;                                     
        }
        
        private static BsonDocument[] GetPersecutorsUserPipeline()
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new Dictionary<string, string> {
            {"tipo", "administracion"}
            });

            var lookUpAggregation = GetRolesLookUpPipeline();

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$roles"));

            var roleMatchAggregation = MongoDBAggregationExtension.Match(new Dictionary<string, string> {
            {"roles.label", "Fiscal"}
            });

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "name", "$datos.nombre" },
                { "lastName", "$datos.apellido" }
            });

            return new BsonDocument[] { matchAggregation, lookUpAggregation, unWindAggregation, roleMatchAggregation, projectAggregation };
        }

        private static BsonDocument GetRolesLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "roleId", new BsonDocument("$toObjectId", "$rol") }
            };

            var lookUpPipeline = new BsonArray()
            {
                 MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                        .Eq(new BsonArray { "$_id", "$$roleId" })))
            };
               

            return MongoDBAggregationExtension.Lookup(new("roles", letPipeline, lookUpPipeline, "roles"));
        } 
       
    }
}
