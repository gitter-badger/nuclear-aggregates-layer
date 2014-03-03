using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class CyprusOrderFileKindEnumCustomization : EnumCustomizationBase<OrderFileKind>, ICyprusAdapted
    {
        protected override IEnumerable<OrderFileKind> GetRequiredEnumValues()
        {
            return new[]
                {
                    OrderFileKind.OrderScan,
                    OrderFileKind.TerminationScan,
                    OrderFileKind.OtherDocumentScan
                };
        }
    }
}