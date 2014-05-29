using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonDtoService : GetLegalPersonDtoServiceBase<UkraineLegalPersonDomainEntityDto>, IUkraineAdapted
    {
        public UkraineGetLegalPersonDtoService(IUserContext userContext,
                                               IClientReadModel clientReadModel,
                                               ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, clientReadModel, legalPersonReadModel)
        {
        }

        protected override IProjectSpecification<LegalPerson, UkraineLegalPersonDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersons.Ukraine.Project.DomainEntityDto();
        }
    }
}