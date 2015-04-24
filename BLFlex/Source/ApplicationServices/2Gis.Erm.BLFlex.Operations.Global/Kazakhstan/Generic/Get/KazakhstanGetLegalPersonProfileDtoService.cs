using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Get
{
    public class KazakhstanGetLegalPersonProfileDtoService : GetLegalPersonProfileDtoServiceBase<KazakhstanLegalPersonProfileDomainEntityDto>, IKazakhstanAdapted
    {
        public KazakhstanGetLegalPersonProfileDtoService(IUserContext userContext,
                                                      ILegalPersonReadModel legalPersonReadModel)
            : base(userContext, legalPersonReadModel)
        {
        }

        protected override IProjectSpecification<LegalPersonProfile, KazakhstanLegalPersonProfileDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersonProfiles.Kazakhstan.Project.DomainEntityDto();
        }

        protected override void SetSpecificPropertyValuesForNewDto(KazakhstanLegalPersonProfileDomainEntityDto dto)
        {
            dto.PaymentMethod = PaymentMethod.BankTransaction;
        }
    }
}