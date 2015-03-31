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

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class PrintJointBillHandler : RequestHandler<PrintJointBillRequest, Response>
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
                                            ProfileId = x.Order.LegalPersonProfileId
                                        })
                                    .FirstOrDefault();

            if (commonInfo == null || commonInfo.BranchOfficeOrganizationUnitId == null)
                throw new NotificationException(BLResources.CannotChoosePrintformBecauseBranchOfficeOrganizationUnitIsNotSpecified);

            if (commonInfo.ProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(commonInfo.BranchOfficeId));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(commonInfo.ProfileId.Value));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(commonInfo.LegalPersonId.Value));

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
                        DestOrganizationUnit = bill.Order.DestOrganizationUnit.Name
                    },
                    bill.Order.Bargain,
                })
                .ToArray();

            var summary = billsInfo.Aggregate(new SummaryJointBillInfo { PayableWithoutVatPlan = 0m, VatPlan = 0m, PayablePlan = 0m },
                                              (sum, item) =>
                                              {
                                                  sum.PayableWithoutVatPlan += item.Bill.PayableWithoutVatPlan;
                                                  sum.VatPlan = (item.Bill.VatPlan.HasValue ? sum.VatPlan + item.Bill.VatPlan.Value : sum.VatPlan);
                                                  sum.PayablePlan += item.Bill.PayablePlan;
                                                  sum.PaymentDatePlan = item.Bill.PaymentDatePlan;
                                                  return sum;
                                              });

            var printData = new
            {
                BranchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(commonInfo.BranchOfficeOrganizationUnitId.Value)),
                BranchOffice = branchOffice,
                LegalPerson = legalPerson,
                Profile = legalPersonProfile,
                commonInfo.CurrencyISOCode,
                BillDate = DateTime.Now,
                commonInfo.PaymentDatePlan,
                commonInfo.BillsBeginDistributionDate,
                commonInfo.BillsEndDistributionDate,
                commonInfo.OrderReleaseCountPlan,
                Bills = billsInfo.Select(b => new
                {
                    b.Bill,
                    commonInfo.OrderReleaseCountPlan,
                    OrderVatRate = (b.Order.VatRate == default(decimal)) ? (decimal?)null : b.Order.VatRate,
                    b.Order,
                    RelatedBargainInfo = (b.Bargain != null) ?
                      string.Format(BLResources.RelatedToBargainInfoTemplate, b.Bargain.Number, _longDateFormatter.Format(b.Bargain.CreatedOn)) : null,
                    commonInfo.BillsBeginDistributionDate,
                    commonInfo.BillsEndDistributionDate
                }),
                Summary = new
                {
                    summary.PayableWithoutVatPlan,
                    summary.VatPlan,
                    summary.PayablePlan,
                    summary.PaymentDatePlan
                }
            };

            const String distributionPeriodDateTemplate = "yyyy_MM_dd";
            var commonDistributionPeriod = commonInfo.BillsBeginDistributionDate.ToString(distributionPeriodDateTemplate) + '-' +
                                           commonInfo.BillsEndDistributionDate.ToString(distributionPeriodDateTemplate);

            return _requestProcessor.HandleSubRequest(new PrintDocumentRequest()
            {
                CurrencyIsoCode = printData.CurrencyISOCode,
                FileName = string.Format(BLResources.JointBill, commonDistributionPeriod),
                BranchOfficeOrganizationUnitId = commonInfo.BranchOfficeOrganizationUnitId,
                TemplateCode = GetTemplateCode(commonInfo.LegalPersonType),
                PrintData = printData
            }, Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.JointBillLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.JointBillBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.JointBillNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
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

