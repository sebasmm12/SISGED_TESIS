﻿using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class SolicitorDossierRequestResponse : Entities.Document
    {
        public SolicitorDossierRequestResponseContent Content { get; set; } = default!;

        public List<MediaRegisterDTO> URLAnnex { get; set; } = default!;

        public SolicitorDossierRequestResponse(SolicitorDossierRequestResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public SolicitorDossierRequestResponse() { }
    }

    public class SolicitorDossierRequestResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DateIssue { get; set; } = default!;
        public string SolicitorId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
    }
}
