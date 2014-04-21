using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.LegalPersonAggregate
{
    // FIXME {all, 10.04.2014}: в процессе доработок по EAV - избавиться от этого класса, т.к. LegalPersonReadModel станет не abstract, а скорее sealed
    public sealed class MultiCultureLegalPersonReadModel : LegalPersonReadModel, ICyprusAdapted, ICzechAdapted, IRussiaAdapted
    {
        public MultiCultureLegalPersonReadModel(IFinder finder, ISecureFinder secureFinder)
            : base(finder, secureFinder)
        {
        }
    }
}
