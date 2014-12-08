using System.Linq;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public class ReleaseStartingAsPreviousAllowedConditionSet : IReleaseStartingOptionConditionSet
    {
        public ReleaseStartingOption EvaluateStartingOption(bool isBeta, ReleaseInfo[] releases, out ReleaseInfo previousRelease)
        {
            previousRelease = null;

            var releasesWaitingForExternalProcessing = releases.Where(x => x.Status == ReleaseStatus.InProgressWaitingExternalProcessing).ToArray();
            if (releasesWaitingForExternalProcessing.Count() > 2 ||
                releasesWaitingForExternalProcessing.Count(x => x.IsBeta) > 1 ||
                releasesWaitingForExternalProcessing.Count(x => !x.IsBeta) > 1)
            {
                return ReleaseStartingOption.Denied | ReleaseStartingOption.BecauseOfInconsistency;
            }

            var releaseWaitingForExternalProcessing = releasesWaitingForExternalProcessing.SingleOrDefault(x => x.IsBeta == isBeta);
            if (releaseWaitingForExternalProcessing != null)
            {
                previousRelease = releaseWaitingForExternalProcessing;
                return ReleaseStartingOption.Allowed | ReleaseStartingOption.AsPrevious;
            }

            return ReleaseStartingOption.Undifined;
        }
    }
}