using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonProfileDtoService : GetLegalPersonProfileDtoServiceBase<UkraineLegalPersonProfileDomainEntityDto>, IUkraineAdapted
    {
        public UkraineGetLegalPersonProfileDtoService(IUserContext userContext,
                                                      ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, legalPersonReadModel)
        {
        }

        protected override IProjectSpecification<LegalPersonProfile, UkraineLegalPersonProfileDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersonProfiles.Ukraine.Project.DomainEntityDto();
        }
    }
}