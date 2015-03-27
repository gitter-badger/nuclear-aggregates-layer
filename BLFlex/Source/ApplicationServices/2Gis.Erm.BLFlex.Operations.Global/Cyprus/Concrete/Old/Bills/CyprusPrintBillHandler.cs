using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Bills
{
    public sealed class CyprusPrintBillHandler : RequestHandler<PrintBillRequest, Response>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public CyprusPrintBillHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
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
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
            {
                throw new NotificationException(BLResources.SpecifiedBillNotFound);
            }

            if (billInfo.LegalPersonProfileId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            if (billInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(Resources.Server.Properties.BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }

            var printData = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                   .Select(bill => new
                                       {
                                           Bill = new
                                               {
                                                   BillNumber = bill.BillNumber,
                                                   OrderReleaseCountPlan = billInfo.OrderReleaseCountPlan,
                                                   BeginDistributionDate = bill.BeginDistributionDate,
                                                   EndDistributionDate = bill.EndDistributionDate,
                                                   PayablePlan = bill.PayablePlan,
                                                   VatPlan = bill.VatPlan,
                                                   PaymentDatePlan = bill.PaymentDatePlan,
                                                   BillDate = bill.BillDate,
                                                   PayableWithoutVatPlan = bill.PayablePlan - bill.VatPlan,
                                                   NoVatText = bill.VatPlan != default(decimal) ? string.Empty : BLResources.NoVatText,
                                                   CreatedOn = bill.CreatedOn,
                                               },
                                           Order = new
                                               {
                                                   bill.Order.Number,
                                                   bill.Order.SignupDate,
                                                   bill.Order.PaymentMethod,
                                                   bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                               },
                                           bill.Order.Bargain,
                                           bill.Order.BranchOfficeOrganizationUnitId,
                                           bill.Order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                           bill.Order.LegalPersonId,
                                           CurrencyISOCode = bill.Order.Currency.ISOCode
                                       })
                                   .ToArray()
                                   .Select(x => new
                                       {
                                           x.Bill,
                                           OrderVatRate = (x.Order.VatRate == default(decimal)) 
                                                ? (decimal?)null 
                                                : x.Order.VatRate,
                                           x.Order,
                                           PaymentMethod = (x.Order.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture),
                                           RelatedBargainInfo = (x.Bargain != null)
                                                ? string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, _longDateFormatter.Format(x.Bargain.CreatedOn))
                                                : null,
                                           billInfo.OrderReleaseCountPlan,
                                           BranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnitId.HasValue 
                                               ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(x.BranchOfficeOrganizationUnitId.Value)) 
                                               : null,
                                           BranchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(x.BranchOfficeId)),
                                           LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(x.LegalPersonId.Value)),
                                           Profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(billInfo.LegalPersonProfileId.Value)),
                                           x.CurrencyISOCode
                                       })
                                   .Single();

            var printRequest = new PrintDocumentRequest()
                {
                    CurrencyIsoCode = printData.CurrencyISOCode,
                    FileName = printData.Bill.BillNumber,
                    BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                    PrintData = printData,
                    TemplateCode = GetTemplateCode(billInfo.LegalPersonType)
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.BillLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.BillBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.BillNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}
