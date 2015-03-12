using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings
{
    public interface IWarmClientProcessingSettings : ISettings
    {
        int WarmClientDaysCount { get; }
    }
}