using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _shortDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderBargainHandler(IFinder finder, IFormatterFactory formatterFactory, ISubRequestProcessor requestProcessor)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var data =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               order.Bargain,
                               order.LegalPersonId,
                               ProfileId = order.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value).Id,
                               OrganizationUnitName = order.DestOrganizationUnit.Name,
                               order.BranchOfficeOrganizationUnit.BranchOfficeId,
                               CurrencyIsoCode = order.Currency.ISOCode,
                               order.BranchOfficeOrganizationUnitId,
                           })
                       .Single();

            var branchOfficeOrganizationUnit = data.BranchOfficeOrganizationUnitId.HasValue
                                                   ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(data.BranchOfficeOrganizationUnitId.Value))
                                                   : null;
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.ProfileId));

            var printData = new
                {
                    data.Bargain,
                    LegalPerson = legalPerson,
                    Profile = legalPersonProfile,
                    data.OrganizationUnitName,
                    BranchOfficeOrganizationUnit = branchOfficeOrganizationUnit,
                    BranchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(data.BranchOfficeId)),
                    data.CurrencyIsoCode,
                    LegalPersonType = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                    data.BranchOfficeOrganizationUnitId,
                    OperatesOnTheBasisInGenitive = GetOperatesOnTheBasisInGenitive(legalPersonProfile, (LegalPersonType)legalPerson.LegalPersonTypeEnum),
                };

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            CurrencyIsoCode = printData.CurrencyIsoCode,
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            TemplateCode = GetTemplateCode(printData.LegalPersonType),
                            FileName = printData.Bargain.Number,
                            PrintData = printData
                        },
                    Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.BargainLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.BargainBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.BargainNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }

        private string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
            {
                switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Undefined:
                    case OperatesOnTheBasisType.None:
                        operatesOnTheBasisInGenitive = string.Empty;
                        break;
                    case OperatesOnTheBasisType.Charter:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCharterTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    case OperatesOnTheBasisType.Certificate:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCertificateTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.CertificateNumber,
                            _shortDateFormatter.Format(profile.CertificateDate));
                        break;
                    case OperatesOnTheBasisType.Warranty:
                        operatesOnTheBasisInGenitive =
                            string.Format(
                                legalPersonType == LegalPersonType.NaturalPerson
                                    ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                                    : BLResources.OperatesOnBasisOfWarantyTemplate,
                                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                profile.WarrantyNumber,
                                _shortDateFormatter.Format(profile.WarrantyBeginDate));
                        break;
                    case OperatesOnTheBasisType.Bargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfBargainTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.BargainNumber,
                            _shortDateFormatter.Format(profile.BargainBeginDate));
                        break;
                    case OperatesOnTheBasisType.FoundingBargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return operatesOnTheBasisInGenitive;
        }
    }
}
