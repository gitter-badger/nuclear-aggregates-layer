using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
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
        private readonly ISubRequestProcessor _requestProcessor;

        public CyprusPrintBillHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.Id))
                                  .Select(bill => new
                                      {
                                          Bill = bill,
                                          bill.OrderId,
                                          OrderReleaseCountPlan = bill.Order.ReleaseCountPlan,
                                          LegalPersonType = (LegalPersonType)bill.Order.LegalPerson.LegalPersonTypeEnum,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
                throw new NotificationException(BLResources.SpecifiedBillNotFound);

            var printData = _finder.Find(Specs.Find.ById<Bill>(request.Id))
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
                                           bill.Order.BranchOfficeOrganizationUnit,
                                           bill.Order.BranchOfficeOrganizationUnit.BranchOffice,
                                           bill.Order.LegalPerson,
                                           Profile =
                                                       bill.Order.LegalPerson.LegalPersonProfiles.FirstOrDefault(
                                                           y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId),
                                           CurrencyISOCode = bill.Order.Currency.ISOCode
                                       })
                                   .AsEnumerable()
                                   .Select(x => new
                                       {
                                           x.Bill,
                                           OrderVatRate = (x.Order.VatRate == default(decimal)) ? (decimal?)null : x.Order.VatRate,
                                           x.Order,
                                           PaymentMethod =
                                                    ((PaymentMethod)x.Order.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager,
                                                                                                             CultureInfo.CurrentCulture),
                                           RelatedBargainInfo = (x.Bargain != null)
                                                                    ? string.Format(BLResources.RelatedToBargainInfoTemplate,
                                                                                    x.Bargain.Number,
                                                                                    PrintFormFieldsFormatHelper.FormatLongDate(x.Bargain.CreatedOn))
                                                                    : null,
                                           billInfo.OrderReleaseCountPlan,
                                           x.BranchOfficeOrganizationUnit,
                                           x.BranchOffice,
                                           x.LegalPerson,
                                           x.Profile,
                                           x.CurrencyISOCode
                                       })
                                   .Single();

            return _requestProcessor.HandleSubRequest(new PrintDocumentRequest()
                {
                    CurrencyIsoCode = printData.CurrencyISOCode,
                    FileName = printData.Bill.BillNumber,
                    BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                    PrintData = printData,
                    TemplateCode = GetTemplateCode(billInfo.LegalPersonType)
                },
                                                      Context);
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
