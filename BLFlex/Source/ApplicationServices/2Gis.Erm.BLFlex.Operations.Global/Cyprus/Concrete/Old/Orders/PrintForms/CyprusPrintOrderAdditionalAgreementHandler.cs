using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Orders.PrintForms
{
    public sealed class CyprusPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly IFormatter _shortDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public CyprusPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
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

            var printData = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        Order = order,
                        order.Bargain,
                        order.LegalPersonId,
                        ProfileId = order.LegalPersonProfileId,
                        OrganizationUnitName = order.LegalPerson.Client.Territory.OrganizationUnit.Name,
                        order.BranchOfficeOrganizationUnit.BranchOfficeId,
                        CurrencyISOCode = order.Currency.ISOCode,
                        LegalPersonType = order.LegalPerson.LegalPersonTypeEnum,
                        order.BranchOfficeOrganizationUnitId,
                    })
                .ToArray()

                // in-memory transformations
                .Select(x =>
                    {
                        if (x.ProfileId == null)
                        {
                            throw new LegalPersonProfileMustBeSpecifiedException();
                        }

                        var profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(x.ProfileId.Value));
                        return new
                            {
                                x.Order,
                                RelatedBargainInfo = (x.Bargain != null)
                                                         ? string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, _longDateFormatter.Format(x.Bargain.CreatedOn))
                                                         : null,
                                NextReleaseDate = x.Order.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth(),
                                LegalPerson = x.LegalPersonId.HasValue ? _finder.FindOne(Specs.Find.ById<LegalPerson>(x.LegalPersonId.Value)) : null,
                                Profile = profile,
                                x.OrganizationUnitName,
                                BranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnitId.HasValue
                                                                   ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(x.BranchOfficeOrganizationUnitId.Value))
                                                                   : null,
                                BranchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(x.BranchOfficeId)),
                                x.CurrencyISOCode,
                                x.LegalPersonType,
                                x.BranchOfficeOrganizationUnitId,
                                OperatesOnTheBasisInGenitive = GetOperatesOnTheBasisInGenitive(profile, x.LegalPersonType)
                            };
                    })
                .Single();

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = printData.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                    TemplateCode = GetTemplateCode(printData.LegalPersonType),
                    FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, printData.Order.Number),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
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

        private string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            if (profile.OperatesOnTheBasisInGenitive != null)
            {
                switch (profile.OperatesOnTheBasisInGenitive)
                {
                    case OperatesOnTheBasisType.Undefined:
                        operatesOnTheBasisInGenitive = string.Empty;
                        break;
                    case OperatesOnTheBasisType.Charter:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCharterTemplate,
                            profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    case OperatesOnTheBasisType.Certificate:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfCertificateTemplate,
                            profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.CertificateNumber,
                            _shortDateFormatter.Format(profile.CertificateDate));
                        break;
                    case OperatesOnTheBasisType.Warranty:
                        operatesOnTheBasisInGenitive =
                            string.Format(
                                legalPersonType == LegalPersonType.NaturalPerson
                                    ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                                    : BLResources.OperatesOnBasisOfWarantyTemplate,
                                profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                profile.WarrantyNumber,
                                _shortDateFormatter.Format(profile.WarrantyBeginDate));
                        break;
                    case OperatesOnTheBasisType.Bargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfBargainTemplate,
                            profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                            profile.BargainNumber,
                            _shortDateFormatter.Format(profile.BargainBeginDate));
                        break;
                    case OperatesOnTheBasisType.FoundingBargain:
                        operatesOnTheBasisInGenitive = string.Format(
                            BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                            profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return operatesOnTheBasisInGenitive;
        }
    }
}