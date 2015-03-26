using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintBindingChangeAgreementHandler : RequestHandler<PrintBindingChangeAgreementRequest, StreamResponse>, IRussiaAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IFormatter _shortDateFormatter;
        private readonly IFinder _finder;

        public PrintBindingChangeAgreementHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
            _finder = finder;
        }

        protected override StreamResponse Handle(PrintBindingChangeAgreementRequest request)
        {
            var order = _finder.FindOne(Specs.Find.ById<Order>(request.OrderId));

            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), request.OrderId);
            }

            if (order.BranchOfficeOrganizationUnitId == null)
            {
                throw new FieldNotSpecifiedException(BLCoreResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (order.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            var currency = _finder.FindOne(Specs.Find.ById<Currency>(order.CurrencyId));
            var bargain = _finder.FindOne(Specs.Find.ById<Bargain>(order.BargainId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(order.LegalPersonId));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(order.LegalPersonProfileId));
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(order.BranchOfficeOrganizationUnitId));
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(branchOfficeOrganizationUnit.BranchOfficeId));
            var firm = _finder.FindOne(Specs.Find.ById<Firm>(order.FirmId));

            var documentData = PrintHelper.AgreementSharedBody(order, legalPerson, legalPersonProfile, branchOfficeOrganizationUnit, _shortDateFormatter);
            var documenSpecificData = PrintHelper.ChangeAgreementSpecificBody(firm);
            var bargainData = PrintHelper.RelatedBrgain(bargain);
            var requisites = PrintHelper.Requisites(legalPerson, legalPersonProfile, branchOffice, branchOfficeOrganizationUnit);

            var printDocumentRequest = new PrintDocumentRequest
            {
                CurrencyIsoCode = currency.ISOCode,
                FileName = string.Format(BLCoreResources.PrintAdditionalAgreementFileNameFormat, order.Number),
                BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId.Value,
                TemplateCode = TemplateCode.BindingChangeAgreement,
                PrintData = PrintData.Concat(documentData, requisites, bargainData, documenSpecificData)
            };

            return (StreamResponse)_requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }
    }
}