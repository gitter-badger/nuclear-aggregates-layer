using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.PrintForms
{
    public class MultiCulturePrintHelper
    {
        private readonly IFormatter _shortDateFormatter;

        public MultiCulturePrintHelper(IFormatterFactory formatterFactory)
        {
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        public string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            if (profile == null || profile.OperatesOnTheBasisInGenitive == null)
            {
                return string.Empty;
            }

            switch (profile.OperatesOnTheBasisInGenitive)
            {
                case OperatesOnTheBasisType.Undefined:
                case OperatesOnTheBasisType.None:
                    return string.Empty;
                case OperatesOnTheBasisType.Charter:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCharterTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                case OperatesOnTheBasisType.Certificate:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCertificateTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.CertificateNumber,
                        _shortDateFormatter.Format(profile.CertificateDate));
                case OperatesOnTheBasisType.Warranty:
                    return string.Format(
                        legalPersonType == LegalPersonType.NaturalPerson
                            ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                            : BLResources.OperatesOnBasisOfWarantyTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.WarrantyNumber,
                        _shortDateFormatter.Format(profile.WarrantyBeginDate));
                case OperatesOnTheBasisType.Bargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfBargainTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.BargainNumber,
                        _shortDateFormatter.Format(profile.BargainBeginDate));
                case OperatesOnTheBasisType.FoundingBargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
