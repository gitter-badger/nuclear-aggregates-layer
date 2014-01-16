using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Enumeration.Cyprus
{
    public class CyprusOperatesOnTheBasisTypesEnumCustomization : EnumCustomizationBase<OperatesOnTheBasisType>, ICyprusAdapted
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
                    OperatesOnTheBasisType.Bargain,
                    OperatesOnTheBasisType.RegistrationCertificate
                };
        }
    }
}