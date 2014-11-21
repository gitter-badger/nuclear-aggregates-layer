using System;

using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public interface IBusinessModelSettings : ISettings
    {
        int SignificantDigitsNumber { get; }
        BusinessModel BusinessModel { get; }
        Type BusinessModelIndicator { get; }
    }
}