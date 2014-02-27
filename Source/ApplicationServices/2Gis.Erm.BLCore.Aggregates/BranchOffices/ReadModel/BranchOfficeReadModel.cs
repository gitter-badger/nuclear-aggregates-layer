using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    public class BranchOfficeReadModel : IBranchOfficeReadModel
    {
        private readonly IFinder _finder;
        private readonly IDynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance> _branchOfficeOrgUnitPropertiesConverter;

        public BranchOfficeReadModel(
            IFinder finder,
            IDynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance> branchOfficeOrgUnitPropertiesConverter)
        {
            _finder = finder;
            _branchOfficeOrgUnitPropertiesConverter = branchOfficeOrgUnitPropertiesConverter;
        }

        public BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var branchOfficeOrganizationUnit = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId)).Single();
                var branchOfficeOrganizationUnitPart = _finder.SingleOrDefault(branchOfficeOrganizationUnitId,
                                                                               _branchOfficeOrgUnitPropertiesConverter.ConvertFromDynamicEntityInstance);
                branchOfficeOrganizationUnit.Parts = new[] { branchOfficeOrganizationUnitPart };

                transactionScope.Complete();

                return branchOfficeOrganizationUnit;
            }
        }

        public BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var branchOfficeOrganizationUnit = _finder.Find(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BySyncCode1C(syncCode1C)).Single();
                var branchOfficeOrganizationUnitPart = _finder.SingleOrDefault(branchOfficeOrganizationUnit.Id,
                                                                               _branchOfficeOrgUnitPropertiesConverter.ConvertFromDynamicEntityInstance);
                branchOfficeOrganizationUnit.Parts = new[] { branchOfficeOrganizationUnitPart };

                transactionScope.Complete();

                return branchOfficeOrganizationUnit;
            }
        }

        public BusinessEntityInstanceDto GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnitPart branchOfficeOrganizationUnitPart)
        {
            return _finder.Single(branchOfficeOrganizationUnitPart, _branchOfficeOrgUnitPropertiesConverter.ConvertToDynamicEntityInstance);
        }
    }
}