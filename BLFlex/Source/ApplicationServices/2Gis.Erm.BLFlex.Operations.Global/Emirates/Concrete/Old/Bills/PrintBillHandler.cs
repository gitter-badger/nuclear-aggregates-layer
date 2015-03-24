using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
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

        public PrintBillHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                  .Select(bill => new
                                      {
                                          bill.OrderId,
                                          BillNumber = bill.Number,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          CurrencyISOCode = bill.Order.Currency.ISOCode,
                                          bill.Order.LegalPersonProfileId,
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
            {
                throw new EntityNotFoundException(typeof(Bill), request.BillId);
            }

            if (billInfo.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            if (billInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(Resources.Server.Properties.BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }

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
                               BillNumber = bill.Number,
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
                    profile.PaymentEssentialElements,
                },
            };
        }
    }
}
