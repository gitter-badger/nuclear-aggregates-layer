using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Bills
{
    public sealed class PrintBillHandler : RequestHandler<PrintBillRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintBillHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                  .Select(bill => new
                                      {
                                          Bill = bill,
                                          bill.OrderId,
                                          OrderReleaseCountPlan = bill.Order.ReleaseCountPlan,
                                          LegalPersonType = bill.Order.LegalPerson.LegalPersonTypeEnum,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          bill.Order.LegalPersonProfileId,
                                          CurrencyISOCode = bill.Order.Currency.ISOCode
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

            var printDataInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                   .Select(bill => new
                                       {
                                           Bill = new
                                               {
                                                   bill.BillNumber,
                                                   bill.BeginDistributionDate,
                                                   bill.PayablePlan,
                                                   bill.VatPlan,
                                                   bill.PaymentDatePlan,
                                                   PayableWithoutVatPlan = bill.PayablePlan - bill.VatPlan,
                                                   NoVatText = bill.VatPlan != default(decimal) ? string.Empty : BLResources.NoVatText,
                                               },
                                           Order = new
                                               {
                                                   bill.Order.Number,
                                                   bill.Order.SignupDate,
                                                   bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                               },
                                           bill.Order.Bargain,
                                           bill.Order.BranchOfficeOrganizationUnitId,
                                           bill.Order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                           bill.Order.LegalPersonId,
                                       })
                                   .ToArray()
                                   .Select(x => new
                                       {
                                           x.Bill,
                                           x.Order,
                                           BranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnitId.HasValue 
                                                ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(x.BranchOfficeOrganizationUnitId.Value))
                                                : null,
                                           BranchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(x.BranchOfficeId)),
                                           LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(x.LegalPersonId.Value)),
                                           Profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(billInfo.LegalPersonProfileId.Value)),

                                           OrderVatRate = (x.Order.VatRate == default(decimal))
                                               ? (decimal?)null
                                               : x.Order.VatRate,
                                           RelatedBargainInfo = (x.Bargain != null)
                                            ? string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, _longDateFormatter.Format(x.Bargain.CreatedOn))
                                            : null,
                                       })
                                   .Single();

            var billPrintData = new PrintData
                                    {
                                        { "BillNumber", printDataInfo.Bill.BillNumber },
                                        { "PaymentDatePlan", printDataInfo.Bill.PaymentDatePlan },
                                        { "BeginDistributionDate", printDataInfo.Bill.BeginDistributionDate },
                                        { "PayableWithoutVatPlan", printDataInfo.Bill.PayableWithoutVatPlan },
                                        { "NoVatText", printDataInfo.Bill.NoVatText },
                                        { "VatPlan", printDataInfo.Bill.VatPlan },
                                        { "PayablePlan", printDataInfo.Bill.PayablePlan }
                                    };

            var orderPrintData = new PrintData
                                     {
                                         { "SignupDate", printDataInfo.Order.SignupDate },
                                         { "Number", printDataInfo.Order.Number }
                                     };

            var branchOfficeOrganizationUnitPrintData = new PrintData
                                                            {
                                                                { "ShortLegalName", printDataInfo.BranchOfficeOrganizationUnit.ShortLegalName },
                                                                { "PhoneNumber", printDataInfo.BranchOfficeOrganizationUnit.PhoneNumber },
                                                                { "PaymentEssentialElements", printDataInfo.BranchOfficeOrganizationUnit.PaymentEssentialElements },
                                                                { "PositionInNominative", printDataInfo.BranchOfficeOrganizationUnit.PositionInNominative },
                                                                { "ChiefNameInNominative", printDataInfo.BranchOfficeOrganizationUnit.ChiefNameInNominative },
                                                                { "ActualAddress", printDataInfo.BranchOfficeOrganizationUnit.ActualAddress }
                                                            };

            var legalPersonPrintData = new PrintData
                                           {
                                               { "LegalName", printDataInfo.LegalPerson.LegalName },
                                               { "LegalAddress", printDataInfo.LegalPerson.LegalAddress },
                                               { "Inn", printDataInfo.LegalPerson.Inn },
                                               { "Kpp", printDataInfo.LegalPerson.Kpp },
                                               { "PassportSeries", printDataInfo.LegalPerson.PassportSeries },
                                               { "PassportNumber", printDataInfo.LegalPerson.PassportNumber },
                                               { "PassportIssuedBy", printDataInfo.LegalPerson.PassportIssuedBy },
                                               { "RegistrationAddress", printDataInfo.LegalPerson.RegistrationAddress },
                                           };

            var profilePrintData = new PrintData
                                       {
                                           { "Phone", printDataInfo.Profile.Phone },
                                           { "PaymentEssentialElements", printDataInfo.Profile.PaymentEssentialElements },
                                       };

            var branchOfficePrintData = new PrintData
                                            {
                                                { "Inn", printDataInfo.BranchOffice.Inn },
                                                { "LegalAddress", printDataInfo.BranchOffice.LegalAddress },
                                            };

            var printData = new PrintData
                                {
                                    { "Bill", billPrintData },
                                    { "Order", orderPrintData },
                                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnitPrintData },
                                    { "LegalPerson", legalPersonPrintData },
                                    { "Profile", profilePrintData },
                                    { "BranchOffice", branchOfficePrintData },
                                    { "OrderVatRate", printDataInfo.OrderVatRate },
                                    { "RelatedBargainInfo", printDataInfo.RelatedBargainInfo },
                                    { "UseLegalPerson", printDataInfo.LegalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson },
                                    { "UseBusinessman", printDataInfo.LegalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman },
                                    { "UseNaturalPerson", printDataInfo.LegalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson },
                                };

            var printRequest = new PrintDocumentRequest
                                   {
                                       CurrencyIsoCode = billInfo.CurrencyISOCode,
                                       FileName = printDataInfo.Bill.BillNumber,
                                       BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                                       PrintData = printData,
                                       TemplateCode = TemplateCode.BillLegalPerson
                                   };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }
    }
}