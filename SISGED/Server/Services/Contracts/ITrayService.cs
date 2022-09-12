﻿using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Services.Contracts
{
    public interface ITrayService : IGenericService
    {
        Task<InputOutputTrayResponse> GetAsync(string user);
        Task<InputTrayResponse> GetInputStrayAsync(string user);
        Task RegisterUserTrayAsync(string type, string userId);
        Task UpdateTrayForDerivationAsync(UpdateTrayDTO updateTrayDTO);
    }
}
