using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services
{
    public class DocumentoService
    {
        //private readonly IMongoCollection<Document> _documentos;
        //private readonly IMongoCollection<Dossier> _expedientes;
        //private readonly IMongoCollection<Bandeja> _bandejas;
        //private readonly ExpedienteService _expedienteservice;
        //private readonly DocumentoService _documentoservice;
        //public DocumentoService(ISysgedDatabaseSettings settings, ExpedienteService expedienteService)
        //{
        //    var client = new MongoClient(settings.ConnectionString);
        //    var database = client.GetDatabase(settings.DatabaseName);
        //    _documentos = database.GetCollection<Document>("documentos");
        //    _expedientes = database.GetCollection<Dossier>("expedientes");
        //    _bandejas = database.GetCollection<Bandeja>("bandejas");
        //    _expedienteservice = expedienteService;
        //}
    }
}
