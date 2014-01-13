using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AccountObtainer : IBusinessModelEntityObtainer<Account>, IAggregateReadModel<Account>
    {
        private readonly IFinder _finder;

        public AccountObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Account ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AccountDomainEntityDto)domainEntityDto;

            var account =
                dto.Id == 0
                    ? new Account { IsActive = true }
                    : _finder.Find(Specs.Find.ById<Account>(dto.Id)).Single();

            account.BranchOfficeOrganizationUnitId = dto.BranchOfficeOrganizationUnitRef.Id.Value;
            account.LegalPersonId = dto.LegalPersonRef.Id.Value;
            account.Timestamp = dto.Timestamp;

            return account;
        }
    }
}