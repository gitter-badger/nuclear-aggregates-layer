using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class RussiaGetLegalPersonProfileDtoService : GetLegalPersonProfileDtoServiceBase<RussiaLegalPersonProfileDomainEntityDto>, IRussiaAdapted
    {
        public RussiaGetLegalPersonProfileDtoService(IUserContext userContext, 
                                                    ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, legalPersonReadModel)
        {
        }

        protected override MapSpecification<LegalPersonProfile, RussiaLegalPersonProfileDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersonProfiles.Russia.Project.DomainEntityDto();
        }
    }
}