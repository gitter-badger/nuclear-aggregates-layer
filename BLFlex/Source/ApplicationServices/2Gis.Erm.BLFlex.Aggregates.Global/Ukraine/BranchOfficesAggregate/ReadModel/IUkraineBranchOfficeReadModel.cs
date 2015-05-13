using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.BranchOfficesAggregate.ReadModel
{
    public interface IUkraineBranchOfficeReadModel : IAggregateReadModel<BranchOffice>, IUkraineAdapted
    {
        bool AreThereAnyActiveEgrpouDuplicates(long branchOfficeId, string egrpou);
        bool AreThereAnyActiveIpnDuplicates(long branchOfficeId, string ipn);
    }
}