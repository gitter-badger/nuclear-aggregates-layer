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

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderBargainHandler(IFinder finder, ISubRequestProcessor requestProcessor)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var printData =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(
                           order =>
                           new
                               {
                                   order.Bargain,
                                   order.LegalPerson,
                                   Profile =
                               order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                                   y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value),
                                   OrganizationUnitName = order.DestOrganizationUnit.Name,
                                   order.BranchOfficeOrganizationUnit,
                                   order.BranchOfficeOrganizationUnit.BranchOffice,
                                   CurrencyIsoCode = order.Currency.ISOCode,
                                   LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                                   order.BranchOfficeOrganizationUnitId,
                                   OperatesOnTheBasisInGenitive = string.Empty
                               })
                       .AsEnumerable()
                       .Select(
                           x =>
                           new
                               {
                                   x.Bargain,
                                   x.LegalPerson,
                                   x.Profile,
                                   x.OrganizationUnitName,
                                   x.BranchOfficeOrganizationUnit,
                                   x.BranchOffice,
                                   x.CurrencyIsoCode,
                                   x.LegalPersonType,
                                   x.BranchOfficeOrganizationUnitId,
                                   OperatesOnTheBasisInGenitive = GetOperatesOnTheBasisInGenitive(x.Profile, x.LegalPersonType),
                               })
                       .Single();

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

        private static string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
            {
                switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Underfined:
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
                            PrintFormFieldsFormatHelper.FormatShortDate(profile.CertificateDate));
                        break;
                    case OperatesOnTheBasisType.Warranty:
                        operatesOnTheBasisInGenitive =
                            string.Format(
                                legalPersonType == LegalPersonType.NaturalPerson
                                    ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                                    : BLResources.OperatesOnBasisOfWarantyTemplate,
                                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                profile.WarrantyNumber,
                                PrintFormFieldsFormatHelper.FormatShortDate(profile.WarrantyBeginDate));
                        break;
                    case OperatesOnTheBasisType.Bargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfBargainTemplate,
                            ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.BargainNumber,
                            PrintFormFieldsFormatHelper.FormatShortDate(profile.BargainBeginDate));
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
