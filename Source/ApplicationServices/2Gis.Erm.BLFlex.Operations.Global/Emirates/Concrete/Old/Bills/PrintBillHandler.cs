using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.Bills
{
    public sealed class PrintBillHandler : RequestHandler<PrintBillRequest, Response>, IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IPrintValidationOperationService _validationService;

        public PrintBillHandler(ISubRequestProcessor requestProcessor, IFinder finder, IPrintValidationOperationService validationService)
        {
            _finder = finder;
            _validationService = validationService;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            _validationService.ValidateOrder(request.BillId);
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                  .Select(bill => new
                                      {
                                          bill.OrderId,
                                          bill.BillNumber,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          CurrencyISOCode = bill.Order.Currency.ISOCode,
                                          bill.Order.LegalPersonProfileId,
                                      })
                                  .SingleOrDefault();

            var printRequest = new PrintDocumentRequest()
                {
                    CurrencyIsoCode = billInfo.CurrencyISOCode,
                    FileName = billInfo.BillNumber,
                    BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.BillId, billInfo.LegalPersonProfileId.Value),
                    TemplateCode = TemplateCode.BillLegalPerson,
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private object GetPrintData(long billId, long profileId)
        {
            var printData = _finder.Find(Specs.Find.ById<Bill>(billId))
                       .Select(bill => new
                       {
                           Bill = new
                           {
                               bill.BillNumber,
                               bill.BillDate,
                               bill.PayablePlan,
                               bill.PaymentDatePlan,
                               bill.BeginDistributionDate,
                               bill.EndDistributionDate,
                           },

                           Order = new
                           {
                               bill.Order.Number,
                               bill.Order.SignupDate,
                           },

                           BranchOfficeOrganizationUnit = new
                           {
                               bill.Order.BranchOfficeOrganizationUnit.ShortLegalName,
                               bill.Order.BranchOfficeOrganizationUnit.PostalAddress,
                               bill.Order.BranchOfficeOrganizationUnit.PhoneNumber,
                               bill.Order.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                               bill.Order.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                           },

                           BranchOffice = new
                           {
                               bill.Order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                           },

                           LegalPerson = new
                           {
                               bill.Order.LegalPerson.LegalName,
                               bill.Order.LegalPerson.LegalAddress,
                           }
                       })
                       .Single();

            var profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(profileId));

            return new
            {
                printData.Bill,
                printData.Order,
                printData.BranchOfficeOrganizationUnit,
                printData.BranchOffice,
                printData.LegalPerson,

                Profile = new
                {
                    profile.PostAddress,
                    profile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone,
                    profile.BankName,
                    profile.SWIFT,
                    profile.IBAN,
                    profile.AdditionalPaymentElements,
                },
            };
        }
    }
}
