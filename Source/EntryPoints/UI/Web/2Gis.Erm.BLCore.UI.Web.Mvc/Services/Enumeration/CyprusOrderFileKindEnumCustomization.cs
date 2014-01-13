using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
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