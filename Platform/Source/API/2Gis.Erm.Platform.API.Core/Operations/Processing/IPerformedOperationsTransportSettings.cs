using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing
{
    public interface IPerformedOperationsTransportSettings : ISettings
    {
        PerformedOperationsTransport OperationsTransport { get; }
    }
}