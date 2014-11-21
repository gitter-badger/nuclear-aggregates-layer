using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing
{
    public interface IPerformedOperationsTransportSettings : ISettings
    {
        PerformedOperationsTransport OperationsTransport { get; }
    }
}