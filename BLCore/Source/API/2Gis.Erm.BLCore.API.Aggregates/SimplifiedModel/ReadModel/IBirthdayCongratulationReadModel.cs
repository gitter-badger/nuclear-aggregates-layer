using System;

using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel
{
    public interface IBirthdayCongratulationReadModel : ISimplifiedModelConsumerReadModel
    {
        bool IsTherePlannedCongratulation(DateTime congratulationDate);
    }
}