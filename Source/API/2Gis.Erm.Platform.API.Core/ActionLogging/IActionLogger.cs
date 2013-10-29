using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    public interface IActionLogger : ISimplifiedModelConsumer
    {
        IDictionary<string, Tuple<object, object>> LogChanges(EntityName entityType, long entityId, object originalObject, object modifiedObject);
        void LogChanges(EntityName entityType, long entityId, IDictionary<string, Tuple<object, object>> differences);
    }
}