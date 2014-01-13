using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class OperatesOnTheBasisTypesEnumCustomization : EnumCustomizationBase<OperatesOnTheBasisType>, IRussiaAdapted
    {
        protected override IEnumerable<OperatesOnTheBasisType> GetRequiredEnumValues()
        {
            return new[]
        {
                    OperatesOnTheBasisType.Underfined,
                    OperatesOnTheBasisType.Charter,
                    OperatesOnTheBasisType.Certificate,
                    OperatesOnTheBasisType.Warranty,
                    OperatesOnTheBasisType.FoundingBargain,
                    OperatesOnTheBasisType.Bargain
                };
        }
    }
}