using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetLegalPersonDtoService : GetLegalPersonDtoServiceBase<EmiratesLegalPersonDomainEntityDto>, IEmiratesAdapted
    {
        public EmiratesGetLegalPersonDtoService(IClientReadModel clientReadModel,
                                                ILegalPersonReadModel legalPersonReadModel,
                                                IUserContext userContext,
                                                IDealReadModel dealReadModel)
            : base(userContext, clientReadModel, legalPersonReadModel, dealReadModel)
        {
        }

        protected override IProjectSpecification<LegalPerson, EmiratesLegalPersonDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersons.Emirates.Project.DomainEntityDto();
        }
    }
}