using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
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
    public class CzechPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IPrintValidationOperationService _validationService;

        public CzechPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFinder finder, IPrintValidationOperationService validationService)
        {
            _finder = finder;
            _validationService = validationService;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            _validationService.ValidateOrder(request.OrderId);
            var orderInfoValidation =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new
                        {
                            WorkflowStep = (OrderState)order.WorkflowStepId,
                            order.IsTerminated, 
                            order.RejectionDate,
                            order.LegalPersonProfileId,
                        })
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

            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               OrderNumber = order.Number,
                               CurrencyISOCode = order.Currency.ISOCode,
                               LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                               order.BranchOfficeOrganizationUnitId,
                           })
                       .Single();

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    TemplateCode = GetTemplateCode(orderInfo.LegalPersonType, request.PrintType),
                    FileName = string.Format(BLCoreResources.PrintAdditionalAgreementFileNameFormat, orderInfo.OrderNumber),
                    PrintData = GetPrintData(request.OrderId, orderInfoValidation.LegalPersonProfileId.Value)
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        protected PrintData GetPrintData(long orderId, long legalPersonProfileId)
        {
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = order,
                                      order.LegalPersonId,
                                      order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                  })
                              .Single();

            var branchOfficeOrganizationUnit = _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrderId(orderId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));

            return new PrintData
                {
                    { "BranchOffice", CzechPrintHelper.BranchOfficeFields(_finder.FindOne(Specs.Find.ById<BranchOffice>(data.BranchOfficeId))) },
                    { "BranchOfficeOrganizationUnit", CzechPrintHelper.BranchOfficeOrganizationUnitFieldsForAdditionalAgreement(branchOfficeOrganizationUnit) },
                    { "LegalPerson", CzechPrintHelper.LegalPersonFields(legalPerson) },
                    { "Profile", CzechPrintHelper.LegalPersonProfileFieldsForAdditionalAgreement(legalPersonProfile) },
                    { "Order", CzechPrintHelper.OrderFields(data.Order) },
                    { "OperatesOnTheBasis", GetOperatesOnTheBasisString(legalPersonProfile) },
                };
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