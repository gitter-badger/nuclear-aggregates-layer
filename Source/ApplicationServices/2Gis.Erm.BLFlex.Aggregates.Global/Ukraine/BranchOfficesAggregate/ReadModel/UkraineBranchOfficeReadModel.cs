using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.BranchOfficesAggregate.ReadModel
{
    public class UkraineBranchOfficeReadModel : BranchOfficeReadModel, IUkraineBranchOfficeReadModel, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<UkraineBranchOfficePart> _branchOfficePartConverter;

        public UkraineBranchOfficeReadModel(IFinder finder, ISecureFinder secureFinder, IBusinessEntityPropertiesConverter<UkraineBranchOfficePart> branchOfficePartConverter)
            : base(finder, secureFinder)
        {
            _finder = finder;
            _branchOfficePartConverter = branchOfficePartConverter;
        }

        public override BranchOffice GetBranchOffice(long branchOfficeId)
        {
            return _finder.GetEntityWithPart(Specs.Find.ById<BranchOffice>(branchOfficeId), _branchOfficePartConverter);
        }

        public override IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOffice branchOffice)
        {
            return branchOffice.Parts.Cast<UkraineBranchOfficePart>().Select(part => _finder.Single(part, _branchOfficePartConverter));
        }

        public bool AreThereAnyActiveEgrpouDuplicates(long branchOfficeId, string egrpou)
        {
            return _finder.Find(BranchOfficeSpecs.Find.DuplicatesByEgrpou(branchOfficeId, egrpou) && Specs.Find.ActiveAndNotDeleted<BranchOffice>()).Any();
        }

        public bool AreThereAnyActiveIpnDuplicates(long branchOfficeId, string ipn)
        {
            var duplicatesQuery = _finder.Find(BusinessEntitySpecs.BusinessEntity.Find.ByProperty(IpnIdentity.Instance.Id, ipn))
                             .Where(x => x.EntityId != branchOfficeId).Select(x => x.EntityId);

            return _finder.FindAll<BranchOffice>().Join(duplicatesQuery, x => x.Id, y => y.Value, (x, y) => x).Any(x => x.IsActive && !x.IsDeleted);
        }
    }
}
