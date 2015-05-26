using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonDtoService : GetLegalPersonDtoServiceBase<UkraineLegalPersonDomainEntityDto>, IUkraineAdapted
    {
        public UkraineGetLegalPersonDtoService(IUserContext userContext,
                                               IClientReadModel clientReadModel,
                                               ILegalPersonReadModel legalPersonReadModel,
                                               IDealReadModel dealReadModel)
            : base(userContext, clientReadModel, legalPersonReadModel, dealReadModel)
        {
        }

        protected override ProjectSpecification<LegalPerson, UkraineLegalPersonDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersons.Ukraine.Project.DomainEntityDto();
        }
    }
}