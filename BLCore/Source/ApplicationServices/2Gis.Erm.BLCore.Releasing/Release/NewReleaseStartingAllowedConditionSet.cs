using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public class NewReleaseStartingAllowedConditionSet : IReleaseStartingOptionConditionSet
    {
        public ReleaseStartingOption EvaluateStartingOption(bool isBeta, IReadOnlyCollection<ReleaseInfo> releases, out ReleaseInfo previousRelease)
        {
            previousRelease = null;

            if (releases.All(x => (x.IsBeta && x.Status == ReleaseStatus.Success) ||
                                  x.Status == ReleaseStatus.Error ||
                                  x.Status == ReleaseStatus.Reverted))
            {
                return ReleaseStartingOption.Allowed | ReleaseStartingOption.New;
            }

            return ReleaseStartingOption.Undifined;
        }
    }
}