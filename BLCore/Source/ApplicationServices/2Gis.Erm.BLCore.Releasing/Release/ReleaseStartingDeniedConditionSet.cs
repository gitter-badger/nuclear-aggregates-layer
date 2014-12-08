using System.Linq;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public class ReleaseStartingDeniedConditionSet : IReleaseStartingOptionConditionSet
    {
        public ReleaseStartingOption EvaluateStartingOption(bool isBeta, ReleaseInfo[] releases, out ReleaseInfo previuosRelease)
        {
            previuosRelease = null;

            var finalSuccessRelease = releases.SingleOrDefault(x => !x.IsBeta && x.Status == ReleaseStatus.Success);
            if (finalSuccessRelease != null)
            {
                previuosRelease = finalSuccessRelease;
                return ReleaseStartingOption.Denied | ReleaseStartingOption.BecauseOfFinal;
            }

            var runningRelease = releases.SingleOrDefault(x => x.Status == ReleaseStatus.InProgressInternalProcessingStarted ||
                                                               x.Status == ReleaseStatus.Reverting);
            if (runningRelease != null)
            {
                previuosRelease = runningRelease;
                return ReleaseStartingOption.Denied | ReleaseStartingOption.BecauseOfLock;
            }

            return ReleaseStartingOption.Undifined;
        }
    }
}