using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Web.Mvc.Services.Enums
{
    public interface IEnumItemsCache
    {
        IEnumerable<EnumItem> GetOrAdd(Type enumType);
    }
}
