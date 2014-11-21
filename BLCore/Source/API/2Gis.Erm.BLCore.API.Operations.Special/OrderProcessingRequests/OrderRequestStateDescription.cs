namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    public interface IOrderRequestStateDescription
    {
        long RequestId { get; set; }

        string State { get; set; }
    }
}
