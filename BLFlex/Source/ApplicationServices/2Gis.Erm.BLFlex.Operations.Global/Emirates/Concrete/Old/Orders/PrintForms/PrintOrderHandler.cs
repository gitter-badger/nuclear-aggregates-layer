using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, IEmiratesAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel; 
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly OrderPrintFormDataExtractor _orderPrintFormDataExtractor;
        private readonly IFirmReadModel _firmReadModel;

        public PrintOrderHandler(ISubRequestProcessor requestProcessor,
                                 ILegalPersonReadModel legalPersonReadModel,
                                 IBranchOfficeReadModel branchOfficeReadModel,
                                 IOrderPrintFormReadModel orderPrintFormReadModel,
                                 IFormatterFactory formatterFactory,
                                 ISecurityServiceUserIdentifier userIdentifierService,
                                 IFirmReadModel firmReadModel)
        {
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderPrintFormReadModel = orderPrintFormReadModel;
            _firmReadModel = firmReadModel;
            _orderPrintFormDataExtractor = new OrderPrintFormDataExtractor(formatterFactory, userIdentifierService);
        }

        protected override StreamResponse Handle(PrintOrderRequest request)
        {
            var orderInfo = _orderPrintFormReadModel.GetOrderRelationsDto(request.OrderId);

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyIsoCode,
                    FileName = orderInfo.OrderNumber,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId.Value,
                    TemplateCode = TemplateCode.Order,
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
            var contacts = _firmReadModel.GetFirmContactsByAddresses(order.FirmId);
            var bargain = _orderPrintFormReadModel.GetOrderBargain(request.OrderId);

            var billQuery = _orderPrintFormReadModel.GetBillQuery(request.OrderId);
            var orderQuery = _orderPrintFormReadModel.GetOrderQuery(request.OrderId);
            var branchOfficeQuery = _orderPrintFormReadModel.GetBranchOfficeQuery(request.OrderId);
            var orderPositionQuery = _orderPrintFormReadModel.GetOrderPositionQuery(request.OrderId);
            var firmAddressQuery = _orderPrintFormReadModel.GetFirmAddressQuery(request.OrderId);

            var data = PrintData.Concat(_orderPrintFormDataExtractor.GetOrder(orderQuery),
                                    _orderPrintFormDataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _orderPrintFormDataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                    _orderPrintFormDataExtractor.GetBargain(bargain),
                                    _orderPrintFormDataExtractor.GetLegalPerson(legalPerson),
                                    _orderPrintFormDataExtractor.GetLegalPersonProfile(profile),
                                    _orderPrintFormDataExtractor.GetOrderPositions(orderQuery, orderPositionQuery),
                                    _orderPrintFormDataExtractor.GetPaymentSchedule(billQuery),
                                    _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery),
                                    _orderPrintFormDataExtractor.GetCategories(orderQuery),
                                    _orderPrintFormDataExtractor.GetFirmAddresses(firmAddressQuery, contacts));
            return data;
        }
    }
}