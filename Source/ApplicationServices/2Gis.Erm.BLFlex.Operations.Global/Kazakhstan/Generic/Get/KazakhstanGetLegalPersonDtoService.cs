﻿using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Get
{
    public class KazakhstanGetLegalPersonDtoService : GetLegalPersonDtoServiceBase<KazakhstanLegalPersonDomainEntityDto>, IKazakhstanAdapted
    {
        public KazakhstanGetLegalPersonDtoService(IUserContext userContext,
                                                  IClientReadModel clientReadModel,
                                                  ILegalPersonReadModel legalPersonReadModel,
                                                  IDealReadModel dealReadModel) : base(userContext, clientReadModel, legalPersonReadModel, dealReadModel)
        {
        }

        protected override IProjectSpecification<LegalPerson, KazakhstanLegalPersonDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersons.Kazakhstan.Project.DomainEntityDto();
        }
    }
}