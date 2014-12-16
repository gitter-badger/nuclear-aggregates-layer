using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IReleaseStartingOptionConditionSet
    {
        ReleaseStartingOption EvaluateStartingOption(bool isBeta, IReadOnlyCollection<ReleaseInfo> releases, out ReleaseInfo previousRelease);
    }
}