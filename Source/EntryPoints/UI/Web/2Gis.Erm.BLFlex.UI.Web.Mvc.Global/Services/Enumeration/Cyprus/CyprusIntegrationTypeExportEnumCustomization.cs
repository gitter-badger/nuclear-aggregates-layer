using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public sealed class CyprusIntegrationTypeExportEnumCustomization : EnumCustomizationBase<IntegrationTypeExport>, ICyprusAdapted
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
