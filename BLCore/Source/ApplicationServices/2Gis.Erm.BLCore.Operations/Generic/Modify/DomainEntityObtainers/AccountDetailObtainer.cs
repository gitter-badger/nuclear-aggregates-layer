using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AccountDetailObtainer : IBusinessModelEntityObtainer<AccountDetail>, IAggregateReadModel<Account>
    {
        private readonly IFinder _finder;

        public AccountDetailObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AccountDetail ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AccountDetailDomainEntityDto)domainEntityDto;
            
            bool isNew = dto.IsNew();
            var accountDetail = isNew
                    ? new AccountDetail { IsActive = true }
                    : _finder.Find(Specs.Find.ById<AccountDetail>(dto.Id)).Single();

            if (isNew)
            {
                accountDetail.TransactionDate = dto.TransactionDate;
                accountDetail.OperationTypeId = dto.OperationTypeRef.Id.Value;
                accountDetail.AccountId = dto.AccountRef.Id.Value;
            }
            else
            {
                accountDetail.Timestamp = dto.Timestamp;
            }
            
            accountDetail.Description = dto.Description;
            accountDetail.Amount = dto.Amount;

            return accountDetail;
        }
    }
}