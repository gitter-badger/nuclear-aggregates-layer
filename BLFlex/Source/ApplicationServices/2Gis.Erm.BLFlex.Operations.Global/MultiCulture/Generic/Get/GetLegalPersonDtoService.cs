using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetLegalPersonDtoService : GetLegalPersonDtoServiceBase<LegalPersonDomainEntityDto>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public GetLegalPersonDtoService(IUserContext userContext, IClientReadModel clientReadModel, ILegalPersonReadModel legalPersonReadModel, IDealReadModel dealReadModel)
            : base(userContext, clientReadModel, legalPersonReadModel, dealReadModel)
        {
        }

        protected override ProjectSpecification<LegalPerson, LegalPersonDomainEntityDto> GetProjectSpecification()
            {
            return LegalPersonFlexSpecs.LegalPersons.MultiCulture.Project.DomainEntityDto();
        }
    }
}