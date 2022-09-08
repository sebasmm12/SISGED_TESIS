using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SISGED.Server.Helpers;
using SISGED.Server.Services;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        //private readonly DocumentoService _documentoservice;
        //private readonly ExpedienteService _expedienteservice;
        //private readonly BandejaService _bandejaService;
        //private readonly EscriturasPublicasService _escrituraspublicasservice;
        //private readonly IFileStorage _almacenadorDeDocs;
        //private readonly AsistenteService asistenteService;

        //public DocumentosController(DocumentoService documentoservice, IFileStorage almacenadorDeDocs,
        //    ExpedienteService expedienteservice, EscriturasPublicasService escrituraspublicasservice, BandejaService bandejaService,
        //    AsistenteService asistenteService)
        //{
        //    _documentoservice = documentoservice;
        //    _almacenadorDeDocs = almacenadorDeDocs;
        //    _expedienteservice = expedienteservice;
        //    _escrituraspublicasservice = escrituraspublicasservice;
        //    _bandejaService = bandejaService;
        //    this.asistenteService = asistenteService;
        //}
    }
}
