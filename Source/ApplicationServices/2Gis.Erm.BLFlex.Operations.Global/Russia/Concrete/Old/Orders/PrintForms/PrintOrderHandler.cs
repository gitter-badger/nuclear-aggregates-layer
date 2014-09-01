﻿using System.Globalization;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, IRussiaAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel; 
        private readonly IOrderPrintFormReadModel _orderPrintFormReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;

        public PrintOrderHandler(ISubRequestProcessor requestProcessor,
                                 ILegalPersonReadModel legalPersonReadModel,
                                 IBranchOfficeReadModel branchOfficeReadModel,
                                 IOrderPrintFormReadModel orderPrintFormReadModel,
                                 IOrderPrintFormDataExtractor orderPrintFormDataExtractor)
        {
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderPrintFormReadModel = orderPrintFormReadModel;
            _orderPrintFormDataExtractor = orderPrintFormDataExtractor;
        }

        protected override StreamResponse Handle(PrintOrderRequest request)
        {
            var orderInfo = _orderPrintFormReadModel.GetOrderRelationsDto(request.OrderId);

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLFlexResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            var templateCode = GetTemplateCode(request, orderInfo);
            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyIsoCode,
                    FileName = orderInfo.OrderNumber,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId.Value,
                    TemplateCode = templateCode,
                    PrintData = GetPrintData(request, orderInfo, templateCode)
                };

            var response = (StreamResponse)_requestProcessor.HandleSubRequest(printDocumentRequest, Context);
            return response;
        }

        private TemplateCode GetTemplateCode(PrintOrderRequest request, OrderRelationsDto orderInfo)
        {
            var withDiscount = _orderPrintFormReadModel.GetOrderDicount(request.OrderId) > 0;
            switch (_orderPrintFormReadModel.GetOrderContributionType(orderInfo.SourceOrganizationUnitId))
            {
                case ContributionTypeEnum.Branch:
                    return withDiscount ? TemplateCode.OrderWithVatWithDiscount : TemplateCode.OrderWithVatWithoutDiscount;

                case ContributionTypeEnum.Franchisees:
                    return withDiscount ? TemplateCode.OrderWithoutVatWithDiscount : TemplateCode.OrderWithoutVatWithoutDiscount;

                default:
                    var message = string.Format(CultureInfo.CurrentCulture, BLFlexResources.ContributionTypeIsNotSet, orderInfo.SourceOrganizationUnitId);
                    throw new NotificationException(message);
            }
        }

        private PrintData GetPrintData(PrintOrderRequest request, OrderRelationsDto order, TemplateCode templateCode)
        {
            var profileId = order.LegalPersonProfileId.Value;
            var legalPerson = _legalPersonReadModel.GetLegalPerson(order.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(profileId);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId.Value);

            var billQuery = _orderPrintFormReadModel.GetBillQuery(request.OrderId);
            var orderQuery = _orderPrintFormReadModel.GetOrderQuery(request.OrderId);
            var branchOfficeQuery = _orderPrintFormReadModel.GetBranchOfficeQuery(request.OrderId);
            var orderPositionQuery = _orderPrintFormReadModel.GetOrderPositionQuery(request.OrderId);

            return PrintData.Concat(_orderPrintFormDataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _orderPrintFormDataExtractor.GetOrder(orderQuery),
                                    _orderPrintFormDataExtractor.GetOrderPositions(orderQuery, orderPositionQuery),
                                    _orderPrintFormDataExtractor.GetPaymentSchedule(billQuery),
                                    _orderPrintFormDataExtractor.GetUngrouppedFields(orderQuery, boou, legalPerson, profile, templateCode),
                                    _orderPrintFormDataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                    _orderPrintFormDataExtractor.GetLegalPersonProfile(profile));
        }
    }
}