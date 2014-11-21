using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Russia
{
    public class RussiaOperatesOnTheBasisTypesEnumCustomization : EnumCustomizationBase<OperatesOnTheBasisType>, IRussiaAdapted
    {
        protected override IEnumerable<OperatesOnTheBasisType> GetRequiredEnumValues()
        {
            return new[]
        {
                    OperatesOnTheBasisType.Undefined,
                    OperatesOnTheBasisType.Charter,
                    OperatesOnTheBasisType.Certificate,
                    OperatesOnTheBasisType.Warranty,
                    OperatesOnTheBasisType.FoundingBargain,
                    OperatesOnTheBasisType.Bargain
                };
        }
    }
}