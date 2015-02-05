using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, IKazakhstanAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;
        private readonly ILocalizationSettings _localizationSettings;

        public PrintOrderHandler(ISubRequestProcessor requestProcessor,
                                 ILegalPersonReadModel legalPersonReadModel,
                                 IBranchOfficeReadModel branchOfficeReadModel,
                                 IOrderPrintFormReadModel orderPrintFormReadModel,
                                 IOrderPrintFormDataExtractor orderPrintFormDataExtractor,
                                 ILocalizationSettings localizationSettings)
        {
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderPrintFormReadModel = orderPrintFormReadModel;
            _orderPrintFormDataExtractor = orderPrintFormDataExtractor;
            _localizationSettings = localizationSettings;
        }

        protected override StreamResponse Handle(PrintOrderRequest request)
        {
            var orderInfo = _orderPrintFormReadModel.GetOrderRelationsDto(request.OrderId);

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            var printDocumentRequest = new PrintDocumentRequest
                                           {
                                               CurrencyIsoCode = orderInfo.CurrencyIsoCode,
                                               FileName = orderInfo.OrderNumber,
                                               BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId.Value,
                                               TemplateCode = orderInfo.IsOrderWithDiscount ? TemplateCode.OrderWithoutVatWithDiscount : TemplateCode.OrderWithoutVatWithoutDiscount,
                                               PrintData = GetPrintData(request, orderInfo)
                                           };

            var response = (StreamResponse)_requestProcessor.HandleSubRequest(printDocumentRequest, Context);
            return response;
        }

        private PrintData GetPrintData(PrintOrderRequest request, OrderRelationsDto order)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(order.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(order.LegalPersonProfileId.Value);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId.Value);
            var branchOffice = _branchOfficeReadModel.GetBranchOffice(order.BranchOfficeId);
            var bargain = _orderPrintFormReadModel.GetOrderBargain(request.OrderId);

            var billQuery = _orderPrintFormReadModel.GetBillQuery(request.OrderId);
            var orderQuery = _orderPrintFormReadModel.GetOrderQuery(request.OrderId);
            var orderPositionQuery = _orderPrintFormReadModel.GetOrderPositionQuery(request.OrderId);

            var data = PrintData.Concat(_orderPrintFormDataExtractor.GetOrder(orderQuery),
                                        _orderPrintFormDataExtractor.GetBranchOfficeData(branchOffice),
                                        _orderPrintFormDataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                        _orderPrintFormDataExtractor.GetBargain(bargain),
                                        _orderPrintFormDataExtractor.GetLegalPersonData(legalPerson),
                                        _orderPrintFormDataExtractor.GetLegalPersonProfile(profile),
                                        _orderPrintFormDataExtractor.GetOrderPositions(orderQuery, orderPositionQuery),
                                        _orderPrintFormDataExtractor.GetPaymentSchedule(billQuery),
                                        _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery),
                                        new PrintData
                                            {
                                                { "AuthorityDocument", PrintHelper.GetAuthorityDocumentDescription(profile, _localizationSettings.ApplicationCulture) },
                                            });
            return data;
        }
    }
}