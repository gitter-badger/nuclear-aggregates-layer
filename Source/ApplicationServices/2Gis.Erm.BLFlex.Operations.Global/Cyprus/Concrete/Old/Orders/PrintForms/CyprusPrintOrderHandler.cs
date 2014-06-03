using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Orders.PrintForms
{
    public sealed class CyprusPrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, ICyprusAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IFirmRepository _firmAggregateRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;

        public CyprusPrintOrderHandler(ISubRequestProcessor requestProcessor,
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
            var profileId = request.LegalPersonProfileId.HasValue ? request.LegalPersonProfileId.Value : order.MainLegalPersonProfileId;
            var legalPerson = _legalPersonReadModel.GetLegalPerson(order.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(profileId);
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
                                    _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery, legalPerson, profile));
        }
    }
}