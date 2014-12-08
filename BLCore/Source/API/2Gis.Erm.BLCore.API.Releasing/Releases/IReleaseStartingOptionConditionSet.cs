using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IReleaseStartingOptionConditionSet
    {
        ReleaseStartingOption EvaluateStartingOption(bool isBeta, ReleaseInfo[] releases, out ReleaseInfo previousRelease);
    }
}