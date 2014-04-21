using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.BranchOfficesAggregate.ReadModel
{
    public class MultiCultureBranchOfficeReadModel : BranchOfficeReadModel, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public MultiCultureBranchOfficeReadModel(IFinder finder, ISecureFinder secureFinder)
            : base(finder, secureFinder)
        {
        }
    }
}
