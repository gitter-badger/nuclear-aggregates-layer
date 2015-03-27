using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, StreamResponse>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _shortDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override StreamResponse Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var order = _finder.FindOne(Specs.Find.ById<Order>(request.OrderId));

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
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }
            
            var currency = _finder.FindOne(Specs.Find.ById<Currency>(order.CurrencyId));
            var bargain = _finder.FindOne(Specs.Find.ById<Bargain>(order.BargainId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(order.LegalPersonId));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(order.LegalPersonProfileId));
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(order.BranchOfficeOrganizationUnitId));
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(branchOfficeOrganizationUnit.BranchOfficeId));

            var documentData = PrintHelper.AgreementSharedBody(order, legalPerson, legalPersonProfile, branchOfficeOrganizationUnit, _shortDateFormatter);
            var documenSpecificData = PrintHelper.TerminationAgreementSpecificBody(order);
            var bargainData = PrintHelper.RelatedBrgain(bargain);

            var printDocumentRequest = new PrintDocumentRequest
                                           {
                                               CurrencyIsoCode = currency.ISOCode,
                                               FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, order.Number),
                                               BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                                               TemplateCode = TemplateCode.AdditionalAgreementLegalPerson,
                                               PrintData =
                                                   PrintData.Concat(documentData,
                                                                    bargainData,
                                                                    documenSpecificData,
                                                                    PrintHelper.DetermineRequisitesType(legalPerson.LegalPersonTypeEnum),
                                                                    PrintHelper.LegalPersonRequisites(legalPerson),
                                                                    PrintHelper.LegalPersonProfileRequisites(legalPersonProfile),
                                                                    PrintHelper.BranchOfficeRequisites(branchOffice),
                                                                    PrintHelper.BranchOfficeOrganizationUnitRequisites(branchOfficeOrganizationUnit))
                                           };

            return (StreamResponse)_requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }
    }
}
