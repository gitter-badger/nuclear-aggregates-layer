using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Chile
{
    public sealed class ChileIntegrationTypeImportEnumCustomization : EnumCustomizationBase<IntegrationTypeImport>, IChileAdapted
    {
        private readonly IntegrationTypeImport[] _integrationTypes1C = 
            {
                IntegrationTypeImport.AccountDetailsFrom1C,
            };

        protected override IEnumerable<IntegrationTypeImport> GetRequiredEnumValues()
        {
            return Enum.GetValues(typeof(IntegrationTypeImport)).Cast<IntegrationTypeImport>().Except(_integrationTypes1C);
        }
    }
}
