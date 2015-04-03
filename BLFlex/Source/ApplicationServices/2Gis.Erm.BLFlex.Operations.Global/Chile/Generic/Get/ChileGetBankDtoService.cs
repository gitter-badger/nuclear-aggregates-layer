using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public sealed class ChileGetBankDtoService : GetDomainEntityDtoServiceBase<Bank>, IChileAdapted
    {
        private readonly IBankReadModel _readModel;

        public ChileGetBankDtoService(IUserContext userContext, IBankReadModel readModel)
            : base(userContext)
        {
            _readModel = readModel;
        }

        protected override IDomainEntityDto<Bank> GetDto(long entityId)
        {
            var bank = _readModel.GetBank(entityId);
            return new BankDomainEntityDto
                {
                    Id = bank.Id,
                    Name = bank.Name,
                    Timestamp = bank.Timestamp,
                    CreatedByRef = new EntityReference { Id = bank.CreatedBy },
                    ModifiedByRef = new EntityReference { Id = bank.ModifiedBy },
                    CreatedOn = bank.CreatedOn,
                    ModifiedOn = bank.ModifiedOn,
                    IsActive = bank.IsActive,
                    IsDeleted = bank.IsDeleted,
                };
        }

        protected override IDomainEntityDto<Bank> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new BankDomainEntityDto();
        }
    }
}
