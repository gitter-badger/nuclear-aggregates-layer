﻿using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetLegalPersonDtoService : GetLegalPersonDtoServiceBase<LegalPersonDomainEntityDto>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public GetLegalPersonDtoService(IUserContext userContext, IClientReadModel clientReadModel, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, clientReadModel, legalPersonReadModel)
        {
        }

        protected override IProjectSpecification<LegalPerson, LegalPersonDomainEntityDto> GetProjectSpecification()
            {
            return LegalPersonFlexSpecs.LegalPersons.MultiCulture.Project.DomainEntityDto();
        }
    }
}