﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Cyprus.EnumCustomization
{
    public sealed class IntegrationTypeExportEnumCustomization : EnumCustomizationBase<IntegrationTypeExport>, ICyprusAdapted
    {
        private readonly IntegrationTypeExport[] _integrationTypes1C = 
            {
                IntegrationTypeExport.LegalPersonsTo1C,
                IntegrationTypeExport.AccountDetailsTo1C,
                IntegrationTypeExport.AccountDetailsToServiceBus,
            };

        protected override IEnumerable<IntegrationTypeExport> GetRequiredEnumValues()
        {
            return Enum.GetValues(typeof(IntegrationTypeExport)).Cast<IntegrationTypeExport>().Except(_integrationTypes1C);
        }
    }
}
