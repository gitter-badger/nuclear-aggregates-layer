using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetLegalPersonProfileDtoService : GetLegalPersonProfileDtoServiceBase<LegalPersonProfileDomainEntityDto>, ICyprusAdapted, ICzechAdapted
    {
        public GetLegalPersonProfileDtoService(IUserContext userContext, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, legalPersonReadModel)
        {
        }

        protected override IProjectSpecification<LegalPersonProfile, LegalPersonProfileDomainEntityDto> GetProjectSpecification()
                              {
            return LegalPersonFlexSpecs.LegalPersonProfiles.MultiCulture.Project.DomainEntityDto();
        }
    }
}