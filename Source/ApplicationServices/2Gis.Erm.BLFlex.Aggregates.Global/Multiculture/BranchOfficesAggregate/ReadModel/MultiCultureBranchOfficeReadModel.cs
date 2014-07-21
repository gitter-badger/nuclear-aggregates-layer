using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.BranchOfficesAggregate.ReadModel
{
    // TODO {all, 07.07.2014}: Избавиться от этого
    public class MultiCultureBranchOfficeReadModel : BranchOfficeReadModel, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IEmiratesAdapted
    {
        public MultiCultureBranchOfficeReadModel(IFinder finder)
            : base(finder)
        {
        }
    }
}