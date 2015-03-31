using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
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
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Orders.Print
{
    public class PrintCancellationAgreementOperationService : IPrintCancellationAgreementOperationService, IRussiaAdapted
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPublicService _publicService;
        private readonly IFormatter _shortDateFormatter;

        private readonly IOrderReadModel _orderReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public PrintCancellationAgreementOperationService(
            IPublicService publicService, 
            IFormatterFactory formatterFactory, 
            IOperationScopeFactory scopeFactory, 
            IOrderReadModel orderReadModel, 
            IBranchOfficeReadModel branchOfficeReadModel)
        {
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        public PrintFormDocument Print(long orderId)
        {
            return new PrintOperationBuilder()
                .UseScope(_scopeFactory.CreateNonCoupled<PrintCancellationAgreementIdentity>)
                .UseTemplate(TemplateCode.AdditionalAgreementLegalPerson)
                .UseRequest(() => CreateRequest(orderId))
                .UseRequestProcessor(_publicService.Handle)
                .Execute();
        }

        private PrintDocumentRequest CreateRequest(long orderId)
        {
            var order = _orderReadModel.GetOrderSecure(orderId);

            if (!order.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (order.WorkflowStepId != OrderState.OnTermination && order.WorkflowStepId != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (order.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
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

            var documentData = PrintHelper.AgreementSharedBody(order, legalPerson, legalPersonProfile, branchOfficeOrganizationUnit, _shortDateFormatter);
            var documenSpecificData = PrintHelper.TerminationAgreementSpecificBody(order);
            var bargainData = PrintHelper.RelatedBrgain(bargain);
            var requisites = PrintHelper.Requisites(legalPerson, legalPersonProfile, branchOffice, branchOfficeOrganizationUnit);

            return new PrintDocumentRequest
                       {
                           CurrencyIsoCode = currency.ISOCode, 
                           FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, order.Number), 
                           BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId, 
                           PrintData = PrintData.Concat(documentData, requisites, bargainData, documenSpecificData)
                       };
        }
    }
}