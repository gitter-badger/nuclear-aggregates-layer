using System;

using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel.ReadModel
{
    public sealed class BirthdayCongratulationReadModel : IBirthdayCongratulationReadModel
    {
        private readonly IFinder _finder;

        public BirthdayCongratulationReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public bool IsTherePlannedCongratulation(DateTime congratulationDate)
        {
            return _finder.Find(BirthdayCongratulationSpecs.BirthdayCongratulations.Find.ByCongratulationDate(congratulationDate)).Any();
        }
    }
}
