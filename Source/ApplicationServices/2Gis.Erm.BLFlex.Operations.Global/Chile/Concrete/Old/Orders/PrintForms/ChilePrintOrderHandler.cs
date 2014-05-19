using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Orders.PrintForms
{
    public sealed class ChilePrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, IChileAdapted
    {
        private readonly IBankReadModel _bankReadModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IFirmRepository _firmAggregateRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;

        public ChilePrintOrderHandler(IBankReadModel bankReadModel,
                                      ISubRequestProcessor requestProcessor,
                                      IFirmRepository firmAggregateRepository,
                                      ILegalPersonReadModel legalPersonReadModel,
                                      IBranchOfficeReadModel branchOfficeReadModel,
                                      IOrderPrintFormReadModel orderPrintFormReadModel,
                                      IOrderPrintFormDataExtractor orderPrintFormDataExtractor)
        {
            _bankReadModel = bankReadModel;
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
            var profilePart = profile.Parts.OfType<ChileLegalPersonProfilePart>().Single();
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId.Value);
            var boouPart = boou.Parts.OfType<ChileBranchOfficeOrganizationUnitPart>().Single();
            var bank = profilePart.BankId.HasValue ? _bankReadModel.GetBank(profilePart.BankId.Value) : null;
            var contacts = _firmAggregateRepository.GetFirmContacts(order.FirmId);

            var billQuery = _orderPrintFormReadModel.GetBillQuery(request.OrderId);
            var orderQuery = _orderPrintFormReadModel.GetOrderQuery(request.OrderId);
            var firmAddressQuery = _orderPrintFormReadModel.GetFirmAddressQuery(request.OrderId);
            var branchOfficeQuery = _orderPrintFormReadModel.GetBranchOfficeQuery(request.OrderId);
            var orderPositionQuery = _orderPrintFormReadModel.GetOrderPositionQuery(request.OrderId);

            return PrintData.Concat(_orderPrintFormDataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _orderPrintFormDataExtractor.GetFirmAddresses(firmAddressQuery, contacts),
                                    _orderPrintFormDataExtractor.GetOrder(orderQuery),
                                    _orderPrintFormDataExtractor.GetOrderPositions(orderQuery, orderPositionQuery),
                                    _orderPrintFormDataExtractor.GetPaymentSchedule(billQuery),
                                    _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery),
                                    _orderPrintFormDataExtractor.GetBranchOfficeOrganizationUnit(boou, boouPart),
                                    _orderPrintFormDataExtractor.GetLegalPerson(legalPerson),
                                    _orderPrintFormDataExtractor.GetLegalPersonProfile(profile, profilePart, bank));
        }
    }
}