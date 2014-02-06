using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class ChileOrderFileKindEnumCustomization : EnumCustomizationBase<OrderFileKind>, IChileAdapted
    {
        protected override IEnumerable<OrderFileKind> GetRequiredEnumValues()
        {
            return new[]
                {
                    OrderFileKind.OrderScan,
                    OrderFileKind.TerminationScan,
                    OrderFileKind.LetterOfGuaranteeScan,
                    OrderFileKind.OtherDocumentScan
                };
        }
    }
}