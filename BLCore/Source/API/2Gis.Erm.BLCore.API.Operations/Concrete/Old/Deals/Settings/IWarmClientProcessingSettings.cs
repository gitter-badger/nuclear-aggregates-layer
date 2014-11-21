using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings
{
    public interface IWarmClientProcessingSettings : ISettings
    {
        int WarmClientDaysCount { get; }
    }
}