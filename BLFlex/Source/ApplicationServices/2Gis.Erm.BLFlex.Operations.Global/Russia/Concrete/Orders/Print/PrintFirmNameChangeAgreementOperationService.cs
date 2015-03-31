﻿using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Orders.Print
{
    public class PrintFirmNameChangeAgreementOperationService : IPrintFirmNameChangeAgreementOperationService, IRussiaAdapted
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPublicService _publicService;
        private readonly IFormatter _shortDateFormatter;

        private readonly IOrderReadModel _orderReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public PrintFirmNameChangeAgreementOperationService(
            IPublicService publicService, 
            IFormatterFactory formatterFactory, 
            IOperationScopeFactory scopeFactory, 
            IOrderReadModel orderReadModel, 
            IBranchOfficeReadModel branchOfficeReadModel, 
            IFirmReadModel firmReadModel)
        {
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _firmReadModel = firmReadModel;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        public PrintFormDocument Print(long orderId)
        {
            return new PrintOperationBuilder()
                .UseScope(_scopeFactory.CreateNonCoupled<PrintFirmNameChangeAgreementIdentity>)
                .UseTemplate(TemplateCode.FirmNameChangeAgreement)
                .UseRequest(() => CreateRequest(orderId))
                .UseRequestProcessor(_publicService.Handle)
                .Execute();
        }

        private PrintDocumentRequest CreateRequest(long orderId)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);

            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), orderId);
            }

            if (order.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (order.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var currency = _orderReadModel.GetCurrency(order.CurrencyId);
            var bargain = _orderReadModel.GetBargain(order.BargainId);
            var legalPerson = _orderReadModel.GetLegalPerson(order.LegalPersonId);
            var legalPersonProfile = _orderReadModel.GetLegalPersonProfile(order.LegalPersonProfileId);
            var branchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(order.BranchOfficeOrganizationUnitId);
            var branchOffice = _branchOfficeReadModel.GetBranchOffice(branchOfficeOrganizationUnit.BranchOfficeId);
            var firm = _firmReadModel.GetFirm(order.FirmId);

            var documentData = PrintHelper.AgreementSharedBody(order, legalPerson, legalPersonProfile, branchOfficeOrganizationUnit, _shortDateFormatter);
            var documenSpecificData = PrintHelper.ChangeAgreementSpecificBody(firm);
            var bargainData = PrintHelper.RelatedBrgain(bargain);
            var requisites = PrintHelper.Requisites(legalPerson, legalPersonProfile, branchOffice, branchOfficeOrganizationUnit);

            return new PrintDocumentRequest
                       {
                           CurrencyIsoCode = currency.ISOCode, 
                           FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, order.Number), 
                           BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId.Value, 
                           PrintData = PrintData.Concat(documentData, requisites, bargainData, documenSpecificData)
                       };
        }
    }
}