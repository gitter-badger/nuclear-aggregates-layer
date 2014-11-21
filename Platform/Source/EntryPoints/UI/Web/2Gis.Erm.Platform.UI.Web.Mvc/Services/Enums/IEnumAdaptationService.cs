using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums
{
    public interface IEnumAdaptationService
    {
        IEnumerable<EnumItem> GetEnumValues();
    }

    public interface IEnumAdaptationService<T> : IEnumAdaptationService where T : struct, IConvertible
    {
    }
}
