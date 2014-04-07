using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;
using EnumResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.EnumResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public sealed class CzechPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public CzechPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfoValidation =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = (OrderState)order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
                    .Single();

            if (!orderInfoValidation.IsTerminated)
            {
                throw new NotificationException(BLCoreResources.OrderShouldBeTerminated);
            }

            if (orderInfoValidation.WorkflowStep != OrderState.OnTermination && orderInfoValidation.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLCoreResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfoValidation.RejectionDate == null)
            {
                throw new NotificationException(BLCoreResources.OrderRejectDateFieldIsNotFilled);
            }

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        Order = order,
                        order.Bargain,
                        order.LegalPerson,
                        Profile = order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                            y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId.Value),
                        OrganizationUnitName = order.LegalPerson.Client.Territory.OrganizationUnit.Name,
                        order.BranchOfficeOrganizationUnit,
                        order.BranchOfficeOrganizationUnit.BranchOffice,
                        CurrencyISOCode = order.Currency.ISOCode,
                        LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                        order.BranchOfficeOrganizationUnitId,
                    })
                .Single();

            var printData = new
                {
                    orderInfo.Order,
                    RelatedBargainInfo = (orderInfo.Bargain != null)
                                             ? string.Format(
                                                 BLCoreResources.RelatedToBargainInfoTemplate,
                                                 orderInfo.Bargain.Number,
                                                 _longDateFormatter.Format(orderInfo.Bargain.CreatedOn))
                                             : null,
                    NextReleaseDate = orderInfo.Order.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth(),
                    orderInfo.LegalPerson,
                    orderInfo.Profile,
                    orderInfo.OrganizationUnitName,
                    orderInfo.BranchOfficeOrganizationUnit,
                    orderInfo.BranchOffice,
                    orderInfo.CurrencyISOCode,
                    orderInfo.LegalPersonType,
                    orderInfo.BranchOfficeOrganizationUnitId,
                    OperatesOnTheBasis = GetOperatesOnTheBasisString(orderInfo.Profile)
                };

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            CurrencyIsoCode = printData.CurrencyISOCode,
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            TemplateCode = GetTemplateCode(printData.LegalPersonType, request.PrintType),
                            FileName = string.Format(BLCoreResources.PrintAdditionalAgreementFileNameFormat, printData.Order.Number),
                            PrintData = printData
                        },
                    Context);
        }

        private static string GetOperatesOnTheBasisString(LegalPersonProfile profile)
        {
            if (profile.OperatesOnTheBasisInGenitive != (int)OperatesOnTheBasisType.Warranty)
            {
                return string.Empty;
            }

            if (profile.WarrantyBeginDate == null)
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                BLFlexResources.CzechPrintOrderHandler_OperatesOnTheBasisStringTemplate,
                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                profile.WarrantyBeginDate.Value.ToShortDateString());
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType, PrintAdditionalAgreementTarget printType)
        {
            if (printType == PrintAdditionalAgreementTarget.Bargain)
            {
                switch (legalPersonType)
                {
                    case LegalPersonType.LegalPerson:
                        return TemplateCode.BargainAdditionalAgreementLegalPerson;

                    case LegalPersonType.Businessman:
                        return TemplateCode.BargainAdditionalAgreementBusinessman;
                }   
            }

            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.AdditionalAgreementLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.AdditionalAgreementBusinessman;
            }

            throw new ArgumentOutOfRangeException("legalPersonType");
        }
    }
}