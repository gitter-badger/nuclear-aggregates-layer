using System;
using System.Runtime.Remoting.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class ControlFlowMarkerManager : IFlowMarkerManager
    {
        private const string ControlFlowMarkerKey = "ControlFlowMarker";

        public bool TryGetMarker(out Guid flowMarker)
        {
            flowMarker = Guid.Empty;

            var value = CallContext.LogicalGetData(ControlFlowMarkerKey);
            if (value == null)
            {
                return false;
            }

            flowMarker = (Guid)value;
            return true;
        }

        public void AddMarker(Guid flowMarker)
        {
            var value = CallContext.LogicalGetData(ControlFlowMarkerKey);
            if (value != null)
            {
                throw new InvalidOperationException("CallContext already has associated flow marker");
            }

            CallContext.LogicalSetData(ControlFlowMarkerKey, flowMarker);
        }

        public void ClearMarker(Guid flowMarker)
        {
            var value = CallContext.LogicalGetData(ControlFlowMarkerKey);
            if (value == null)
            {
                return;
            }

            var existingMarker = (Guid)value;
            if (!existingMarker.Equals(flowMarker))
            {
                throw new InvalidOperationException(string.Format("Can't clear marker. Specified flow marker {0} not equal to existing flow marker {1}", flowMarker, existingMarker));
            }

            CallContext.LogicalSetData(ControlFlowMarkerKey, null);
        }
    }
}