using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    public interface IOrderProcessingSettings : ISettings
    {
        int OrderRequestProcessingHoursAmount { get; }
    }
}
