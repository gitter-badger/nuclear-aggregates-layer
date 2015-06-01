using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.Security.API.UserContext;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonProfileDtoService : GetLegalPersonProfileDtoServiceBase<ChileLegalPersonProfileDomainEntityDto>, IChileAdapted
    {
        private readonly IBankReadModel _bankReadModel;

        public ChileGetLegalPersonProfileDtoService(IUserContext userContext, 
                                                    ILegalPersonReadModel legalPersonReadModel,
            IBankReadModel bankReadModel)
            : base(userContext, legalPersonReadModel)
        {
            _bankReadModel = bankReadModel;
        }

        protected override MapSpecification<LegalPersonProfile, ChileLegalPersonProfileDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersonProfiles.Chile.Project.DomainEntityDto();
            }

        protected override void SetSpecificPropertyValues(ChileLegalPersonProfileDomainEntityDto dto)
            {
            if (dto.BankRef.Id.HasValue)
            {
                dto.BankRef.Name = _bankReadModel.GetBankName(dto.BankRef.Id.Value);
            }
        }
    }
}