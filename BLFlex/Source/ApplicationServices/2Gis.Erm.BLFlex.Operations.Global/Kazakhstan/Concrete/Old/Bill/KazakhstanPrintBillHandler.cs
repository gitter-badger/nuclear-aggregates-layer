using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Bill
{
    public sealed class KazakhstanPrintBillHandler : RequestHandler<PrintBillRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;

        public KazakhstanPrintBillHandler(ISubRequestProcessor requestProcessor,
                                          IFinder finder,
                                          ILegalPersonReadModel legalPersonReadModel,
                                          IBranchOfficeReadModel branchOfficeReadModel,
                                          IOrderPrintFormDataExtractor orderPrintFormDataExtractor)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderPrintFormDataExtractor = orderPrintFormDataExtractor;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Bill>(request.BillId))
                                  .Map(q => q.Select(bill => new
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
                                      }))
                                  .One();

            if (billInfo == null)
            {
                throw new NotificationException(BLResources.SpecifiedBillNotFound);
            }

            if (billInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(Resources.Server.Properties.BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }

            var branchOffice = _branchOfficeReadModel.GetBranchOffice(billInfo.BranchOfficeId.Value);
            var legalPerson = _legalPersonReadModel.GetLegalPerson(billInfo.LegalPersonId);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(billInfo.LegalPersonProfileId.Value);
            var orderVatRate = (billInfo.OrderVatRate.Value == default(decimal)) ? BLResources.NoVatText : billInfo.OrderVatRate.ToString();
            var branchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(billInfo.BranchOfficeOrganizationUnitId.Value);

            var printData = new PrintData
                                {
                                    { "BranchOffice", PrintHelper.BranchOfficeFields(branchOffice) },
                                    { "BranchOfficeOrganizationUnit", PrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                                    { "Profile", PrintHelper.LegalPersonProfileFields(profile) },
                                    { "Order", PrintHelper.OrderFields(billInfo.Order) },
                                    { "Bill", GetBillFields(billInfo.Bill) },
                                    { "OrderVatRate", orderVatRate },
                                };

            printData = PrintData.Concat(printData,
                                         _orderPrintFormDataExtractor.GetBargain(billInfo.Bargain),
                                         _orderPrintFormDataExtractor.GetLegalPersonData(legalPerson));

            return _requestProcessor.HandleSubRequest(
                new PrintDocumentRequest
                    {
                        CurrencyIsoCode = billInfo.CurrencyISOCode,
                        FileName = billInfo.Bill.Number,
                        BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                        PrintData = printData,
                        TemplateCode = TemplateCode.BillLegalPerson
                    },
                Context);
        }

        private PrintData GetBillFields(Platform.Model.Entities.Erm.Bill bill)
        {
            return new PrintData
                {
                    { "BillNumber", bill.Number },
                    { "BillDate", bill.BillDate },
                    { "BeginDistributionDate", bill.BeginDistributionDate },
                    { "EndDistributionDate", bill.EndDistributionDate },
                    { "PayablePlan", bill.PayablePlan },
                    { "PayableWithoutVatPlan", bill.PayablePlan - bill.VatPlan },
                    { "PaymentDatePlan", bill.PaymentDatePlan }
                };
        }
    }
}
