using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    public interface IActionLogger : ISimplifiedModelConsumer
    {
        void LogChanges(IEnumerable<ChangesDescriptor> changeDescriptors);
        void LogChanges(ChangesDescriptor changeDescriptor);
    }
}