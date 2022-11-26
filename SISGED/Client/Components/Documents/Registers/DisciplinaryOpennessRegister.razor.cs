using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using SISGED.Client;
using SISGED.Client.Generics;
using SISGED.Client.Shared;
using SISGED.Client.Helpers;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Shared.Models.Responses.Tray;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using MudBlazor;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class DisciplinaryOpennessRegister
    {
        //Variables de sesion
        [CascadingParameter] WorkEnvironment workspace { get; set; }
        //[CascadingParameter(Name = "SesionUsuario")] protected Sesion sesion { get; set; }
        //Datos del formulario
        [Parameter] public EventCallback<DossierTrayResponse> increaseTray { get; set; }
        private DisciplinaryOpennessResponse fdgdfgd = new DisciplinaryOpennessResponse();
        private EditContext _editContext;
        List<string> names = new List<string>();
        private string tempImage;
        private string tempImage2;
        private bool loadProcess = false;
        int step = 0;
        int substep = 0;
        String typeDocument = "AperturamientoDisciplinario";

        List<UserInfoResponse> prosecutors { get; set; } = new List<UserInfoResponse>();

        //protected override async Task OnInitializedAsync()
        //{
        //    fdgdfgd.Content.Participants = new List<Participant>() { new Participant() { Index = 0, Name = "" } };
        //    fdgdfgd.Content.ChargedDeeds = new List<Deed>() { new Deed() { Index = 0, Description = "" } };
        //    fdgdfgd.Content.URLAnnex = new List<string>();
        //    _editContext = new EditContext(fdgdfgd);
        //    foreach (string u in fdgdfgd.Content.URLAnnex)
        //    {
        //        if (!string.IsNullOrWhiteSpace(u))
        //        {
        //            tempImage2 = u;
        //            fdgdfgd.Content.URLAnnex = null;
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(fdgdfgd.Content.URL))
        //    {
        //        tempImage = fdgdfgd.Content.URL;
        //        fdgdfgd.Content.URL = null;
        //    }
        //    var httpResponse = await repository.Get<List<UsuarioRDTO>>("api/usuarios/fiscales");
        //    if (!httpResponse.Error)
        //    {
        //        prosecutors = httpResponse.Response;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Ocurrio un error");
        //    }
        //    substep = 1;
        //    String message = "";

        //    if (typeDocument != workspace.asistente.tipodocumento)
        //    {
        //        message = "El documento que ha elegido no es el indicado para el expediente";
        //    }
        //    else
        //    {
        //        message = workspace.asistente.pasos.documentos.Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(step).subpaso.ElementAt(substep).descripcion;

        //        await workspace.UpdatePasoAndSubPasoNormal(step, substep, workspace.asistente.tipodocumento);
        //    }

        //    Task voiceMessage = workspace.VoiceMessage(message);

        //    workspace.ChangeMessage(message);

        //    await voiceMessage;


        //}

        //private void addparticipante()
        //{
        //    fdgdfgd.contenidoDTO.participantes.Add(new Participante() { index = (fdgdfgd.contenidoDTO.participantes.Count) });
        //    StateHasChanged();
        //}

        //private void removeparticipante(int index)
        //{
        //    fdgdfgd.contenidoDTO.participantes.RemoveAt(index);
        //    StateHasChanged();
        //}
        //private void addhecho()
        //{
        //    fdgdfgd.contenidoDTO.hechosimputados.Add(new Hecho() { index = (fdgdfgd.contenidoDTO.hechosimputados.Count) });
        //    StateHasChanged();
        //}
        //private void removehecho(int index)
        //{
        //    fdgdfgd.contenidoDTO.hechosimputados.RemoveAt(index);
        //    StateHasChanged();
        //}

        //private void ImagenSeleccionada(string imagenbase64)
        //{
        //    fdgdfgd.contenidoDTO.url = imagenbase64;
        //}

        //private void ImagenSeleccionada2(string imagenbase64)
        //{
        //    fdgdfgd.contenidoDTO.Urlanexo.Add(imagenbase64);
        //}

        //private void FileName(string imagenbase64)
        //{
        //    names.Add(imagenbase64);
        //}

        //private async Task RemoveFile(string file, int imagen64)
        //{
        //    names.Remove(file);
        //    fdgdfgd.contenidoDTO.Urlanexo.RemoveAt(imagen64);
        //    StateHasChanged();

        //}

        ////Consulta de notarios
        //private async Task<IEnumerable<Notario>> match(string searchtext)
        //{
        //    var httpResponse = await repository.Get<List<Notario>>($"api/notarios/filter?term={searchtext}");
        //    if (httpResponse.Error)
        //    {
        //        return new List<Notario>();
        //    }
        //    else
        //    {
        //        return httpResponse.Response;
        //    }
        //}

        //void KeyUp(ChangeEventArgs e, string memberName, object val)
        //{
        //    var property = val.GetType().GetProperty(memberName);
        //    property.SetValue(val, e.Value.ToString());
        //    var fi = new FieldIdentifier(val, memberName);
        //    _editContext.NotifyFieldChanged(fi);
        //}

        //public async Task handleValidSubmit()
        //{
        //    loadProcess = true;
        //    if (fdgdfgd.contenidoDTO.url != "" && fdgdfgd.contenidoDTO.url != null)
        //    {
        //        ExpedienteWrapper expedienteWrapper = new ExpedienteWrapper();
        //        expedienteWrapper.documento = fdgdfgd;
        //        expedienteWrapper.idexpediente = workspace.expedienteseleccionado.idexpediente;
        //        expedienteWrapper.idusuarioactual = sesion.usuario.id;
        //        expedienteWrapper.documentoentrada = workspace.expedienteseleccionado.documento.id;

        //        var httpResponse = await repository.Post<ExpedienteWrapper, AperturamientoDisciplinario>($"api/documentos/documentoad", expedienteWrapper);
        //        if (!httpResponse.Error)
        //        {
        //            ExpedienteBandejaDTO expedientebandeja = new ExpedienteBandejaDTO();
        //            expedientebandeja = workspace.expedienteseleccionado;
        //            AperturamientoDisciplinario aperturamientodisciplinarioact = new AperturamientoDisciplinario();
        //            aperturamientodisciplinarioact = httpResponse.Response;
        //            expedientebandeja.documento.id = aperturamientodisciplinarioact.id;
        //            expedientebandeja.documento.tipo = aperturamientodisciplinarioact.tipo;
        //            expedientebandeja.documento.historialcontenido = aperturamientodisciplinarioact.historialcontenido;
        //            expedientebandeja.documento.historialproceso = aperturamientodisciplinarioact.historialproceso;
        //            expedientebandeja.documento.contenido = aperturamientodisciplinarioact.contenido;
        //            expedientebandeja.documento.estado = aperturamientodisciplinarioact.estado;
        //            //expedientebandeja.documentosobj.Add(expedientebandeja.documento);
        //            Item itemSalida = new Item()
        //            {
        //                nombre = expedientebandeja.tipo,
        //                valor = expedientebandeja,
        //                icono = "alarm_add",
        //                descripcion = ((DocumentoDTO)expedientebandeja.documento).tipo,
        //                currentPlace = "workspace",
        //                originPlace = "inputs",
        //                cliente = expedientebandeja.cliente,
        //                itemstatus = "registrado"
        //            };
        //            workspace.UpdateDocRegistrado(itemSalida);
        //            workspace.UpdateTools("Registrar Documento");
        //            DocumentoDTO documentoDTO = new DocumentoDTO();
        //            documentoDTO.id = aperturamientodisciplinarioact.id;
        //            documentoDTO.tipo = aperturamientodisciplinarioact.tipo;
        //            documentoDTO.estado = aperturamientodisciplinarioact.estado;
        //            documentoDTO.contenido = JsonConvert.SerializeObject(aperturamientodisciplinarioact.contenido);
        //            documentoDTO.historialcontenido = aperturamientodisciplinarioact.historialcontenido;
        //            documentoDTO.historialproceso = aperturamientodisciplinarioact.historialproceso;
        //            documentoDTO.urlanexo = aperturamientodisciplinarioact.urlanexo;

        //            var docdto = JsonConvert.SerializeObject(documentoDTO);
        //            Console.WriteLine(docdto);
        //            DocumentoDTO doc = new DocumentoDTO();
        //            doc = JsonConvert.DeserializeObject<DocumentoDTO>(docdto);

        //            workspace.expedienteseleccionado.documentosobj.Add(documentoDTO);
        //            workspace.expedienteseleccionado.documento = documentoDTO;
        //            StateHasChanged();
        //            Console.WriteLine(documentoDTO.ToString());
        //            //workspace.UpdateComponentBandeja(itemSalida);
        //            await swalfire.successMessage("Aperturamiento Disciplinario registrado correctamente");
        //            substep = 2;

        //            Int32 pasoAntiguo = step;
        //            step = 1;
        //            substep = 0;
        //            //Enviar paso=0+0,subpaso=2,idexpediente
        //            String tipodocumentoantiguo = workspace.asistente.tipodocumento;

        //            await workspace.UpdatePasoAndSubPasoFinnally(step, substep, workspace.asistente.tipodocumento, pasoAntiguo, tipodocumentoantiguo);

        //            String messageFinal = workspace.asistente.pasos.documentos
        //                    .Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(step).descripcion;

        //            Task voiceMessage = workspace.VoiceMessage(messageFinal);

        //            workspace.ChangeMessage(messageFinal);

        //            await voiceMessage;
        //            //Enviar paso=0,subpaso=2, idexp
        //        }
        //        else
        //        {
        //            await swalfire.errorMessage("Error en el servidor, intentelo de nuevo");
        //        }
        //    }
        //    else
        //    {
        //        await swalfire.errorMessage("Debe subir un PDF obligatoriamente");
        //    }
        //    loadProcess = false;
        //}

        //public void handleInvalidSubmit()
        //{
        //    loadProcess = false;
        //    swalfire.errorMessage("Por favor, Verifique los Datos Ingresados");
        //}
    }
}