using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class ResourcesEnumLocalizer : IEnumLocalizer
    {
        public string LocalizeFromId<T>(int id)
        {
            var e = (Enum)Enum.ToObject(typeof(T), id);
            return e.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }
    }
}