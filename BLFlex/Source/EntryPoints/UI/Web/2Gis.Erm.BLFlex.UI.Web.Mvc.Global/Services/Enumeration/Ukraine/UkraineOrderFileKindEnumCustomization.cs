﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Ukraine
{
    public class UkraineOrderFileKindEnumCustomization : EnumCustomizationBase<OrderFileKind>, IUkraineAdapted
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