﻿using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services;
using SISGED.Shared.Entities;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarpetaController : ControllerBase
    {
        private readonly CarpetaService _carpetaService;

        public CarpetaController(CarpetaService carpetaService)
        {
            _carpetaService = carpetaService;
        }

        [HttpGet]
        public ActionResult<List<Carpeta>> Get()
        {
            return _carpetaService.Get();
        }
    }
}
