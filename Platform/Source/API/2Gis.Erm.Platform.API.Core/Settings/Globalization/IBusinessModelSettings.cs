using System;

using DoubleGis.Erm.Platform.Model;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public interface IBusinessModelSettings : ISettings
    {
        int SignificantDigitsNumber { get; }
        BusinessModel BusinessModel { get; }
        Type BusinessModelIndicator { get; }
    }
}