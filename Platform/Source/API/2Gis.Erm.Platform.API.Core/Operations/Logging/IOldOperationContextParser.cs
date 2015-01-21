using System;
using System.Collections.Generic;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    [Obsolete("устаревший parser формирует набор фиктивных операций над множествами сущностей, после перехода на транспорт выполненных бизнес операций, удалить")]
    public interface IOldOperationContextParser
    {
        IReadOnlyDictionary<StrictOperationIdentity, IEnumerable<long>> GetGroupedIdsFromContext(string context, int operation, int descriptor);
    }
}