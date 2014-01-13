using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public class CzechOrderFileKindEnumCustomization : EnumCustomizationBase<OrderFileKind>, ICzechAdapted
    {
        protected override IEnumerable<OrderFileKind> GetRequiredEnumValues()
        {
            return new[]
                {
                    OrderFileKind.OrderScan,
                    OrderFileKind.TerminationScan,
                    OrderFileKind.SwornStatementScan, 
                    OrderFileKind.WarrantyScan, 
                    OrderFileKind.OtherDocumentScan
                };
        }
    }
}