using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Bills
{
    public sealed class UkrainePrintBillHandler : RequestHandler<PrintBillRequest, Response>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly UkrainePrintHelper _ukrainePrintHelper;

        public UkrainePrintBillHandler(ISubRequestProcessor requestProcessor,
                                       IFinder finder,
                                       ILegalPersonReadModel legalPersonReadModel,
                                       IBranchOfficeReadModel branchOfficeReadModel,
                                       IFormatterFactory formatterFactory)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _requestProcessor = requestProcessor;
            _ukrainePrintHelper = new UkrainePrintHelper(formatterFactory);
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                  .Select(bill => new
                                      {
                                          Bill = bill,
                                          bill.Order,

                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          CurrencyISOCode = bill.Order.Currency.ISOCode,
                                          BranchOfficeId = (long?)bill.Order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                          LegalPersonId = bill.Order.LegalPersonId.Value,
                                          LegalPersonProfileId = bill.Order.LegalPersonProfileId,
                                          OrderVatRate = (long?)bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                          bill.Order.Bargain,
                                          bill.Order.LegalPerson.LegalPersonTypeEnum
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
            {
                throw new NotificationException(BLResources.SpecifiedBillNotFound);
            }

            if (billInfo.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            if (billInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(Resources.Server.Properties.BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }

            var branchOffice = _branchOfficeReadModel.GetBranchOffice(billInfo.BranchOfficeId.Value);
            var legalPerson = _legalPersonReadModel.GetLegalPerson(billInfo.LegalPersonId);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(billInfo.LegalPersonProfileId.Value);
            var orderVatRate = (billInfo.OrderVatRate.Value == default(decimal)) ? BLResources.NoVatText : billInfo.OrderVatRate.ToString();
            var branchOfficeOrganizationUnit = billInfo.BranchOfficeOrganizationUnitId.HasValue
                ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(billInfo.BranchOfficeOrganizationUnitId.Value))
                : null;

            var printData = new PrintData
                {
                    { "BranchOffice", UkrainePrintHelper.BranchOfficeFields(branchOffice) },
                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(profile) },
                    { "Order", UkrainePrintHelper.OrderFields(billInfo.Order) },
                    { "Bill", GetBillFields(billInfo.Bill) },
                    { "OrderVatRate", orderVatRate },
                    { "RelatedBargainInfo", _ukrainePrintHelper.GetRelatedBargainInfo(billInfo.Bargain) },
                };

            return _requestProcessor.HandleSubRequest(
                new PrintDocumentRequest
                    {
                        CurrencyIsoCode = billInfo.CurrencyISOCode,
                        FileName = billInfo.Bill.Number,
                        BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                        PrintData = printData,
                        TemplateCode = GetTemplateCode(billInfo.LegalPersonTypeEnum)
                    },
                Context);
        }

        private PrintData GetBillFields(Bill bill)
        {
            return new PrintData
                {
                    { "BillNumber", bill.Number },
                    { "BillDate", bill.BillDate },
                    { "PaymentDatePlan", bill.PaymentDatePlan },
                    { "BeginDistributionDate", bill.BeginDistributionDate },
                    { "EndDistributionDate", bill.EndDistributionDate },
                    { "PayablePlan", bill.PayablePlan },
                    { "VatPlan", bill.VatPlan },
                    { "PayableWithoutVatPlan", bill.PayablePlan - bill.VatPlan }
                };
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.BillLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.BillBusinessman;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}
