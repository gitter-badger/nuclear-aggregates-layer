using System;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IFlowMarkerManager
    {
        bool TryGetMarker(out Guid flowMarker);
        void AddMarker(Guid flowMarker);
        void ClearMarker(Guid flowMarker);
    }
}
