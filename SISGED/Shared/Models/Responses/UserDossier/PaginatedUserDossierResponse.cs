﻿using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.UserDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.UserDossier
{
    public class PaginatedUserDossierResponse
    {
        public PaginatedUserDossierResponse(IEnumerable<DossierListResponse> userDossiers, int total)
        {
            UserDossiers = userDossiers;
            Total = total;
        }

        public PaginatedUserDossierResponse() { }

        public IEnumerable<DossierListResponse> UserDossiers { get; set; } = default!;
        public int Total { get; set; }
    }
}