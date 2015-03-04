using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    public interface IOrderProcessingSettings : ISettings
    {
        int OrderRequestProcessingHoursAmount { get; }
    }
}
