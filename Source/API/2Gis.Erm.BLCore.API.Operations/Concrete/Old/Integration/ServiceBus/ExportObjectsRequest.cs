using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    // ReSharper disable UnusedTypeParameter
    public sealed class ExportObjectsRequest<TEntity> : Request
    {
        public string FlowName { get; set; }
        public bool ExportInvalidObjects { get; set; }
    }
    // ReSharper restore UnusedTypeParameter
}
