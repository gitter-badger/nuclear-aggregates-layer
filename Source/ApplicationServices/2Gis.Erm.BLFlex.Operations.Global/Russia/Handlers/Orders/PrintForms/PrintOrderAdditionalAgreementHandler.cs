using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Handlers.Orders.PrintForms
{
    public sealed class PrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfo =
                _finder.Find(GenericSpecifications.ById<Order>(request.OrderId))
                       .Select(order => new { WorkflowStep = (OrderState)order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
                       .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.WorkflowStep != OrderState.OnTermination && orderInfo.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfo.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
            }

            var printData = _finder.Find(GenericSpecifications.ById<Order>(request.OrderId))
                .Select(order => new
                                 {
                                     Order = order,
                                     order.Bargain,
                                     order.LegalPerson,
                                     Profile =
                                     order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                                         y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value),
                                     OrganizationUnitName = order.LegalPerson.Client.Territory.OrganizationUnit.Name,
                                     order.BranchOfficeOrganizationUnit,
                                     order.BranchOfficeOrganizationUnit.BranchOffice,
                                     CurrencyISOCode = order.Currency.ISOCode,
                                     LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                                     order.BranchOfficeOrganizationUnitId,
                                 })
                .AsEnumerable()

                // in-memory transformations
                .Select(x => new
                             {
                                 x.Order,
                                 RelatedBargainInfo = (x.Bargain != null) ? 
                                    string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, PrintFormFieldsFormatHelper.FormatLongDate(x.Bargain.CreatedOn)) : null,
                                 NextReleaseDate = x.Order.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth(),
                                 x.LegalPerson,
                                 x.Profile,
                                 x.OrganizationUnitName,
                                 x.BranchOfficeOrganizationUnit,
                                 x.BranchOffice,
                                 x.CurrencyISOCode,
                                 x.LegalPersonType,
                                 x.BranchOfficeOrganizationUnitId,
                                 OperatesOnTheBasisInGenitive = GetOperatesOnTheBasisInGenitive(x.Profile, x.LegalPersonType)
                             })
                .Single();

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            CurrencyIsoCode = printData.CurrencyISOCode,
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            TemplateCode = GetTemplateCode(printData.LegalPersonType),
                            FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, printData.Order.Number),
                            PrintData = printData
                        },
                    Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.AdditionalAgreementLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.AdditionalAgreementBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.AdditionalAgreementNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }

        private static string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            if (profile.OperatesOnTheBasisInGenitive != null)
            {
                switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Underfined:
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
