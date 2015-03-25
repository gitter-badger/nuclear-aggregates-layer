using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.MultiCulture
{
    public sealed class MultiCultureIntegrationTypeExportEnumCustomization : EnumCustomizationBase<IntegrationTypeExport>, IChileAdapted, ICyprusAdapted,
                                                                             ICzechAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IntegrationTypeExport[] _integrationTypes1C =
            {
                IntegrationTypeExport.LegalPersonsTo1C
            };

        protected override IEnumerable<IntegrationTypeExport> GetRequiredEnumValues()
        {
            return Enum.GetValues(typeof(IntegrationTypeExport)).Cast<IntegrationTypeExport>().Except(_integrationTypes1C);
        }
    }
}