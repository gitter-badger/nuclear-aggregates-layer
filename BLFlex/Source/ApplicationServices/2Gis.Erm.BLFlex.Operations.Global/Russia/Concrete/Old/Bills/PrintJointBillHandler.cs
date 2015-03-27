using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms;
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
    public sealed class PrintJointBillHandler : RequestHandler<PrintJointBillRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintJointBillHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintJointBillRequest request)
        {
            var commonInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                    .Select(x => new
                                                     {
                                                         BillsBeginDistributionDate = x.BeginDistributionDate,
                                                         BillsEndDistributionDate = x.EndDistributionDate,
                                                         x.BillDate,
                                                         x.PaymentDatePlan,
                                                         OrderReleaseCountPlan = x.Order.ReleaseCountPlan,
                                                         x.Order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                                         x.Order.LegalPersonId,
                                                         CurrencyISOCode = x.Order.Currency.ISOCode,
                                                         LegalPersonType = x.Order.LegalPerson.LegalPersonTypeEnum,
                                                         x.Order.BranchOfficeOrganizationUnitId,
                                                         ProfileId = x.Order.LegalPersonProfileId,
                                                         x.Order.SourceOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary)
                                                          .BranchOffice.ContributionTypeId
                                                     })
                                    .FirstOrDefault();

            if (commonInfo == null || commonInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.CannotChoosePrintformBecauseBranchOfficeOrganizationUnitIsNotSpecified);
            }

            if (commonInfo.ProfileId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(commonInfo.BranchOfficeId));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(commonInfo.ProfileId.Value));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(commonInfo.LegalPersonId.Value));
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(commonInfo.BranchOfficeOrganizationUnitId.Value));

            var billsInfo = _finder.Find<Bill>(bill => bill.IsActive && !bill.IsDeleted &&
                                                       (bill.Id == request.BillId ||
                                                        (bill.Id != request.BillId
                                                         && bill.BeginDistributionDate == commonInfo.BillsBeginDistributionDate
                                                         && bill.EndDistributionDate == commonInfo.BillsEndDistributionDate
                                                         && request.RelatedOrdersId.Contains(bill.OrderId))))
                                   .Select(bill => new
                                                       {
                                                           Bill = new BillInfo
                                                                      {
                                                                          BillNumber = bill.BillNumber,
                                                                          BeginDistributionDate = bill.BeginDistributionDate,
                                                                          EndDistributionDate = bill.EndDistributionDate,
                                                                          PayablePlan = bill.PayablePlan,
                                                                          VatPlan = bill.VatPlan,
                                                                          PaymentDatePlan = bill.PaymentDatePlan,
                                                                          PayableWithoutVatPlan = bill.PayablePlan - bill.VatPlan,
                                                                          NoVatText = bill.VatPlan != default(decimal) ? string.Empty : BLResources.NoVatText,
                                                                          CreatedOn = bill.CreatedOn,
                                                                          OrderReleaseCountPlan = bill.Order.ReleaseCountPlan
                                                                      },
                                                           Order = new
                                                                       {
                                                                           bill.Order.Number,
                                                                           bill.Order.SignupDate,
                                                                           bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                                                           DestOrganizationUnit = bill.Order.DestOrganizationUnit.Name,
                                                                           bill.Order.CreatedOn
                                                                       },
                                                           bill.Order.Bargain,
                                                       })
                                   .ToArray();

            var summary = billsInfo.Aggregate(new SummaryJointBillInfo { PayableWithoutVatPlan = 0m, VatPlan = 0m, PayablePlan = 0m },
                                              (sum, item) =>
                                                  {
                                                      sum.PayableWithoutVatPlan += item.Bill.PayableWithoutVatPlan;
                                                      sum.VatPlan = item.Bill.VatPlan.HasValue ? sum.VatPlan + item.Bill.VatPlan.Value : sum.VatPlan;
                                                      sum.PayablePlan += item.Bill.PayablePlan;
                                                      return sum;
                                                  });

            var bills = billsInfo.Select(b => new
                                                  {
                                                      b.Bill,

                                                      OrderVatRate = (b.Order.VatRate == default(decimal)) ? (decimal?)null : b.Order.VatRate,
                                                      b.Order,
                                                      RelatedBargainInfo = (b.Bargain != null)
                                                                               ? string.Format(BLResources.RelatedToBargainInfoTemplate,
                                                                                               b.Bargain.Number,
                                                                                               _longDateFormatter.Format(b.Bargain.CreatedOn))
                                                                               : null,
                                                  });

            var maxOrderCreatedOnDate = bills.Max(x => x.Order.CreatedOn);
            summary.PaymentDatePlan = bills.First(x => x.Order.CreatedOn == maxOrderCreatedOnDate).Bill.PaymentDatePlan;

            var billsPrintData = new PrintData
                                     {
                                         {
                                             "Bills",
                                             bills.Select(x => new PrintData
                                                                   {
                                                                       {
                                                                           "Bill", new PrintData
                                                                                       {
                                                                                           { "PayableWithoutVatPlan", x.Bill.PayableWithoutVatPlan },
                                                                                           { "NoVatText", x.Bill.NoVatText },
                                                                                           { "VatPlan", x.Bill.VatPlan },
                                                                                           { "PayablePlan", x.Bill.PayablePlan }
                                                                                       }
                                                                       },
                                                                       {
                                                                           "Order", new PrintData
                                                                                        {
                                                                                            { "Number", x.Order.Number },
                                                                                            { "SignupDate", x.Order.SignupDate },
                                                                                            { "DestOrganizationUnit", x.Order.DestOrganizationUnit }
                                                                                        }
                                                                       },
                                                                       { "OrderVatRate", x.OrderVatRate },
                                                                       { "BillsBeginDistributionDate", commonInfo.BillsBeginDistributionDate },
                                                                       { "BillsEndDistributionDate", commonInfo.BillsEndDistributionDate },
                                                                       { "RelatedBargainInfo", x.RelatedBargainInfo }
                                                                   }).ToArray()
                                         }
                                     };

            var summaryPrintData = new PrintData
                                       {
                                           {
                                               "Summary", new PrintData
                                                              {
                                                                  { "PaymentDatePlan", summary.PaymentDatePlan },
                                                                  { "PayableWithoutVatPlan", summary.PayableWithoutVatPlan },
                                                                  { "VatPlan", summary.VatPlan },
                                                                  { "PayablePlan", summary.PayablePlan },
                                                              }
                                           }
                                       };

            var printData = new PrintData
                                {
                                    { "BillDate", commonInfo.BillDate },
                                    { "PaymentDatePlan", commonInfo.PaymentDatePlan }
                                };

            const string DistributionPeriodDateTemplate = "yyyy_MM_dd";
            var commonDistributionPeriod = commonInfo.BillsBeginDistributionDate.ToString(DistributionPeriodDateTemplate) + '-' +
                                           commonInfo.BillsEndDistributionDate.ToString(DistributionPeriodDateTemplate);

            return _requestProcessor.HandleSubRequest(new PrintDocumentRequest
                                                          {
                                                              CurrencyIsoCode = commonInfo.CurrencyISOCode,
                                                              FileName = string.Format(BLResources.JointBill, commonDistributionPeriod),
                                                              BranchOfficeOrganizationUnitId = commonInfo.BranchOfficeOrganizationUnitId,
                                                              TemplateCode = TemplateCode.JointBill,
                                                              PrintData = PrintData.Concat(printData,
                                                                                           billsPrintData,
                                                                                           summaryPrintData,
                                                                                           PrintHelper.DetermineBilletType((ContributionTypeEnum)commonInfo.ContributionTypeId),
                                                                                           PrintHelper.DetermineRequisitesType(legalPerson.LegalPersonTypeEnum),
                                                                                           PrintHelper.LegalPersonRequisites(legalPerson),
                                                                                           PrintHelper.LegalPersonProfileRequisites(legalPersonProfile),
                                                                                           PrintHelper.BranchOfficeRequisites(branchOffice),
                                                                                           PrintHelper.BranchOfficeOrganizationUnitRequisites(branchOfficeOrganizationUnit)),
                                                          },
                                                      Context);
        }

        private sealed class SummaryJointBillInfo
        {
            public decimal PayableWithoutVatPlan { get; set; }
            public decimal VatPlan { get; set; }
            public decimal PayablePlan { get; set; }
            public DateTime PaymentDatePlan { get; set; }
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private sealed class BillInfo
        {
            public string BillNumber { get; set; }
            public int OrderReleaseCountPlan { get; set; }
            public DateTime BeginDistributionDate { get; set; }
            public DateTime EndDistributionDate { get; set; }
            public DateTime CreatedOn { get; set; }
            public decimal PayablePlan { get; set; }
            public decimal? VatPlan { get; set; }
            public DateTime PaymentDatePlan { get; set; }
            public decimal PayableWithoutVatPlan { get; set; }
            public string NoVatText { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
