﻿using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.User;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Server.Services.Repositories
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        public string CollectionName => "usuarios";
        public UserService(IMongoDatabase mongoDatabase)
        {
            _usersCollection = mongoDatabase.GetCollection<User>(CollectionName);
        }

        public async Task<List<User>> GetUsersAsync(UserPaginationQuery userPaginationQuery)
        {

            var users = await _usersCollection
                                    .Find(user => true)
                                    .Skip(userPaginationQuery.Page * userPaginationQuery.PageSize)
                                    .Limit(userPaginationQuery.PageSize)
                                    .ToListAsync();

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

        public async Task<User> VerifyUserLoginAsync(string username)
        {
            var user = await _usersCollection.Find(user => user.UserName == username).FirstOrDefaultAsync();

            if (user is null) throw new Exception("El usuario no existe en el sistema");

            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, user.Id);
            var update = SetUserInformation(user);

            var updatedUser = await _usersCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedUser is null) throw new Exception("No se pudo actualizar el usuario");
        }

        public async Task UpdateUserStateAsync(string userId, string state)
        {
            var userState = state == "activo" ? "inactivo" : "activo";

            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            var update = Builders<User>.Update.Set("state", userState);

            var updatedUser = await _usersCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedUser is null) throw new Exception("No se pudo actualizar el usuario");
        }

        public async Task UpdateUserPasswordAsync(string userId, string password)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            var update = Builders<User>.Update.Set("password", password);

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

            if (prosecutorUsers is null) throw new Exception("No se ha podido encontrar los fiscales registrados");

            return prosecutorUsers;
        }

        public async Task<IEnumerable<ClientUserInfoResponse>> GetClientUsersAsync(string userName)
        {
            string userNameRegex = @"\b" + userName + ".*";

            var regexFilter = Builders<User>.Filter.Regex("data.name", new BsonRegularExpression(userNameRegex, "i"));

            var userTypeFilter = Builders<User>.Filter.Eq("type", "cliente");

            var clientProjection = new FindExpressionProjectionDefinition<User, ClientUserInfoResponse>(user => new ClientUserInfoResponse
            {
                Id = user.Id,
                Name = user.Data.Name,
                LastName = user.Data.LastName,
                Profile = user.Data.Profile
            });

            var clientUsers = await _usersCollection.Find(regexFilter & userTypeFilter).Project(clientProjection).ToListAsync();

            if (clientUsers is null) throw new Exception($"No se pudo encontrar los clientes registrados en el sistema con el nombre {userName}");

            return clientUsers;
        }

        public async Task<IEnumerable<AutocompletedUserResponse>> GetAutocompletedUsersAsync(string userName)
        {
            var autocompletedUsers = await _usersCollection.Aggregate<AutocompletedUserResponse>(GetUsersPipeline(userName))
                                                            .ToListAsync();

            if (autocompletedUsers is null) throw new Exception($"No se pudo encontrar los usuarios registrados con el nombre {userName}");

            return autocompletedUsers;
        }

        public async Task<long> CountUsersAsync()
        {
            long totalUsers = await _usersCollection.CountDocumentsAsync(_ => true);

            return totalUsers;
        }
        
        public async Task<UserValidationDTO> ValidateUserRegisterAsync(User user)
        {
            bool hasSameUserName = await _usersCollection.Find(userEntity => userEntity.UserName.ToLower() == user.UserName.ToLower()).AnyAsync();

            if (hasSameUserName) return new(true, "El nombre de usuario ya se encuentra registrado en el sistema");

            bool hasSameDocument = await _usersCollection.Find(userEntity =>
                 userEntity.Data.DocumentType == user.Data.DocumentType &&
                userEntity.Data.DocumentNumber == user.Data.DocumentNumber).AnyAsync();

            if (hasSameDocument) return new(true, "El numero de documento ya se encuentra registrado en el sistema");

            bool hasSameEmail = await _usersCollection.Find(userEntity => userEntity.Data.Email.ToLower() == user.Data.Email.ToLower()).AnyAsync();

            if (hasSameEmail) return new(true, "El correo electronico ya se encuentra registrado en el sistema");

            return new(false, string.Empty);
        }
        

        #region private methods

        private static UpdateDefinition<User> SetUserInformation(User user)
        {
            return Builders<User>.Update.Set("userName", user.UserName)
                                        .Set("password", user.Password)
                                        .Set("data", user.Data)
                                        .Set("rol", user.Rol);
        }
        private static BsonDocument[] GetUsersPipeline(string userName)
        {
            string userNameRegex = @"\b" + userName;

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "type", 1 },
                { "data", 1 },
                { "rol", 1 },
                { "name", MongoDBAggregationExtension.Concat(new List<BsonValue> { "$data.name", " ", "$data.lastName" })}
            });

            var matchAggregation = MongoDBAggregationExtension.Match(new Dictionary<string, BsonValue> {
            { "type", "administracion" },
            { "name", new BsonDocument().Add("$regex", userNameRegex).Add("$options", "si") }
            });

            var lookUpAggregation = GetRolesLookUpPipeline();

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$roles"));

            var autocompletedProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "userName", "$name" },
                { "roleName", "$roles.label" },
                { "document", "$data.documentNumber" }
            });

            return new[] { projectAggregation, matchAggregation, lookUpAggregation, unWindAggregation, autocompletedProjectAggregation };
        }

        private static BsonDocument[] GetPersecutorsUserPipeline()
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new Dictionary<string, BsonValue> {
            {"type", "administracion"}
            });

            var lookUpAggregation = GetRolesLookUpPipeline();

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$roles"));

            var roleMatchAggregation = MongoDBAggregationExtension.Match(new Dictionary<string, BsonValue> {
            {"roles.label", "Fiscal"}
            });

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "name", "$data.name" },
                { "lastName", "$data.lastName" }
            });

            return new[] { matchAggregation, lookUpAggregation, unWindAggregation, roleMatchAggregation, projectAggregation };
        }

        private static BsonDocument GetRolesLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "roleId", MongoDBAggregationExtension.ObjectId("$rol") }
            };

            var lookUpPipeline = new BsonArray()
            {
                 MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                        .Eq(new BsonArray { "$_id", "$$roleId" })))
            };


            return MongoDBAggregationExtension.Lookup(new("roles", letPipeline, lookUpPipeline, "roles"));
        }
        #endregion
    }
}
