using System;
using System.Globalization;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class EnumResourcesEnumLocalizer : IEnumLocalizer
    {
        public string GetLocalizedString(Enum enumId)
        {
            return enumId.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }
    }
}