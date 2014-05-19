using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.BranchOfficesAggregate.ReadModel
{
    public class ChileBranchOfficeReadModel : BranchOfficeReadModel, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> _branchOfficeOrgUnitPropertiesConverter;

        public ChileBranchOfficeReadModel(IFinder finder, ISecureFinder secureFinder, IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> branchOfficeOrgUnitPropertiesConverter)
            : base(finder, secureFinder)
        {
            _finder = finder;
            _branchOfficeOrgUnitPropertiesConverter = branchOfficeOrgUnitPropertiesConverter;
        }

        public override IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return branchOfficeOrganizationUnit.Parts.Cast<ChileBranchOfficeOrganizationUnitPart>().Select(part => _finder.Single(part, _branchOfficeOrgUnitPropertiesConverter));
        }

        public override BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            return _finder.GetEntityWithPart(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId), _branchOfficeOrgUnitPropertiesConverter);
        }

        public override BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C)
        {
            return _finder.GetEntityWithPart(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BySyncCode1C(syncCode1C), _branchOfficeOrgUnitPropertiesConverter);
        }
    }
}
