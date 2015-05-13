using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.BranchOfficesAggregate.ReadModel
{
    // TODO {all, 07.07.2014}: Избавиться от этого
    public class MultiCultureBranchOfficeReadModel : BranchOfficeReadModel, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        public MultiCultureBranchOfficeReadModel(IFinder finder)
            : base(finder)
        {
        }
    }
}