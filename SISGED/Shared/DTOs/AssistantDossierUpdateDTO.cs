﻿using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class AssistantDossierUpdateDTO
    {
        public AssistantDossierUpdateDTO(string id, string dossierType, string documentType, AssistantStep steps, int step)
        {
            Id = id;
            Step = step;
            DossierType = dossierType;
            DocumentType = documentType;
            Steps = steps;
        }

        public string Id { get; set; }
        public int Step { get; set; }
        public string DossierType { get; set; }
        public string DocumentType { get; set; }
        public AssistantStep Steps { get; set; }
    }
}
