using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class CzechOperatesOnTheBasisTypesEnumCustomization : EnumCustomizationBase<OperatesOnTheBasisType>, ICzechAdapted
    {
        protected override IEnumerable<OperatesOnTheBasisType> GetRequiredEnumValues()
        {
            return new[]
                {
                    OperatesOnTheBasisType.Underfined,
                    OperatesOnTheBasisType.Warranty,
                    OperatesOnTheBasisType.None
                };
        }
    }
}