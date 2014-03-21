using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public sealed class CzechPrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, ICzechAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IFirmRepository _firmAggregateRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;

        public CzechPrintOrderHandler(ISubRequestProcessor requestProcessor,
                                      IFirmRepository firmAggregateRepository,
                                      ILegalPersonReadModel legalPersonReadModel,
                                      IBranchOfficeReadModel branchOfficeReadModel,
                                      IOrderPrintFormReadModel orderPrintFormReadModel,
                                      IOrderPrintFormDataExtractor orderPrintFormDataExtractor)
        {
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _firmAggregateRepository = firmAggregateRepository;
            _orderPrintFormReadModel = orderPrintFormReadModel;
            _orderPrintFormDataExtractor = orderPrintFormDataExtractor;
        }

        protected override StreamResponse Handle(PrintOrderRequest request)
        {
            var orderInfo = _orderPrintFormReadModel.GetOrderRelationsDto(request.OrderId);

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLFlexResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyIsoCode,
                    FileName = orderInfo.OrderNumber,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId.Value,
                    TemplateCode = TemplateCode.OrderWithVatWithDiscount,
                    PrintData = GetPrintData(request, orderInfo)
                };

            var response = (StreamResponse)_requestProcessor.HandleSubRequest(printDocumentRequest, Context);
            return response;
        }

        private PrintData GetPrintData(PrintOrderRequest request, OrderRelationsDto order)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(order.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(request.LegalPersonProfileId);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId.Value);
            var contacts = _firmAggregateRepository.GetFirmContacts(order.FirmId);

            var billQuery = _orderPrintFormReadModel.GetBillQuery(request.OrderId);
            var orderQuery = _orderPrintFormReadModel.GetOrderQuery(request.OrderId);
            var firmAddressQuery = _orderPrintFormReadModel.GetFirmAddressQuery(request.OrderId);
            var branchOfficeQuery = _orderPrintFormReadModel.GetBranchOfficeQuery(request.OrderId);
            var orderPositionQuery = _orderPrintFormReadModel.GetOrderPositionQuery(request.OrderId);

            return PrintData.Concat(_orderPrintFormDataExtractor.GetPaymentSchedule(billQuery),
                                    _orderPrintFormDataExtractor.GetLegalPersonProfile(profile),
                                    _orderPrintFormDataExtractor.GetOrder(orderQuery),
                                    _orderPrintFormDataExtractor.GetFirmAddresses(firmAddressQuery, contacts),
                                    _orderPrintFormDataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _orderPrintFormDataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                    _orderPrintFormDataExtractor.GetOrderPositions(orderQuery, orderPositionQuery),
                                    _orderPrintFormDataExtractor.GetLegalPerson(legalPerson),
                                    _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery),
                                    _orderPrintFormDataExtractor.GetClient(legalPerson, profile));
        }
    }
}