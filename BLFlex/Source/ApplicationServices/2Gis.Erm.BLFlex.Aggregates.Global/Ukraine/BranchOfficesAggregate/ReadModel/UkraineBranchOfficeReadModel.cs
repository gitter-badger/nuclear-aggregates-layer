using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.BranchOfficesAggregate.ReadModel
{
    public class UkraineBranchOfficeReadModel : BranchOfficeReadModel, IUkraineBranchOfficeReadModel, IUkraineAdapted
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public UkraineBranchOfficeReadModel(IQuery query, IFinder finder)
            : base(finder)
        {
            _query = query;
            _finder = finder;
        }

        public override BranchOffice GetBranchOffice(long branchOfficeId)
        {
            return _finder.Find(Specs.Find.ById<BranchOffice>(branchOfficeId)).One();
        }

        public bool AreThereAnyActiveEgrpouDuplicates(long branchOfficeId, string egrpou)
        {
            return _finder.Find(BranchOfficeSpecs.Find.DuplicatesByEgrpou(branchOfficeId, egrpou) && Specs.Find.ActiveAndNotDeleted<BranchOffice>()).Any();
        }

        public bool AreThereAnyActiveIpnDuplicates(long branchOfficeId, string ipn)
        {
            var duplicatesQuery = _query.For(BusinessEntitySpecs.BusinessEntity.Find.ByProperty(IpnIdentity.Instance.Id, ipn))
                                         .Where(x => x.EntityId != branchOfficeId).Select(x => x.EntityId);

            return _query.For<BranchOffice>().Join(duplicatesQuery, x => x.Id, y => y.Value, (x, y) => x).Any(x => x.IsActive && !x.IsDeleted);
        }
    }
}
