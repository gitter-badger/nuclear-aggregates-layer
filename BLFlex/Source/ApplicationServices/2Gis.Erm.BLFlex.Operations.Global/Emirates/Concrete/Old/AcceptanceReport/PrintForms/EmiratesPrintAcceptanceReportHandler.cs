using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.AcceptanceReport.PrintForms
{
    public class EmiratesPrintAcceptanceReportHandler : RequestHandler<EmiratesPrintAcceptanceReportRequest, Response>, IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public EmiratesPrintAcceptanceReportHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(EmiratesPrintAcceptanceReportRequest request)
        {
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               OrderNumber = order.Number,
                               FirmName = order.Firm.Name,
                               order.BranchOfficeOrganizationUnitId,
                               order.EndDistributionDateFact,
                               SourceOrganizationUnitName = order.SourceOrganizationUnit.Name,
                               CurrencyIsoCode = order.Currency.ISOCode,
                           })
                       .Single();

            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(orderInfo.EndDistributionDateFact.Month);

            var printRequest = new PrintDocumentRequest
                {
                    TemplateCode = TemplateCode.AcceptanceReport,
                    FileName = string.Format("{0}_{1}_{2}_{3}", orderInfo.OrderNumber, orderInfo.FirmName, orderInfo.SourceOrganizationUnitName, monthName),
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.OrderId),
                    CurrencyIsoCode = orderInfo.CurrencyIsoCode
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        protected PrintData GetPrintData(long orderId)
        {
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = order,
                                      ProfileId = order.LegalPersonProfileId,
                                      LegalPersonName = order.LegalPerson.LegalName,
                                      LegalPersonAddress = order.LegalPerson.LegalAddress,
                                      BranchOfficeOrganizationId = order.BranchOfficeOrganizationUnitId.Value,
                                      BranchOfficeName = order.BranchOfficeOrganizationUnit.BranchOffice.Name,
                                      BranchOfficeAddress = order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                  })
                              .Single();

            if (!data.ProfileId.HasValue)
            {
                throw new OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException(
                    string.Format(BLResources.EmiratesCannotPrintAcceptanceReportSinceProfileIsNotSpecifiedForOrder,
                                  data.Order.Number));
            }

            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.ProfileId.Value));
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(data.BranchOfficeOrganizationId));

            return new PrintData
                {
                    { "AcceptanceReportNumber", string.Format("{0}-AR", data.Order.Number) },
                    { "BranchOfficeOrganizationUnit", GetBranchOfficeOrganizationUnitData(branchOfficeOrganizationUnit) },
                    {
                        "LegalPerson", new PrintData
                            {
                                { "LegalName", data.LegalPersonName },
                                { "LegalAddress", data.LegalPersonAddress },
                            }
                    },
                    { "Order", GetOrderData(data.Order) },
                    { "LegalPersonProfile", GetProfileData(legalPersonProfile) },
                    {
                        "BranchOffice", new PrintData
                            {
                                { "Name", data.BranchOfficeName },
                                { "LegalAddress", data.BranchOfficeAddress },
                            }
                    },
                };
        }

        private PrintData GetOrderData(Order order)
        {
            return new PrintData
                {
                    { "Number", order.Number },
                    { "SignupDate", order.SignupDate },
                    { "EndDistributionDateFact", order.EndDistributionDateFact },
                    { "BeginDistributionDate", order.BeginDistributionDate },
                    { "PayableFact", order.PayableFact },
                };
        }

        private PrintData GetProfileData(LegalPersonProfile profile)
        {
            return new PrintData
                {
                    { "PostAddress", profile.PostAddress },
                    { "Phone", profile.Within<EmiratesLegalPersonProfilePart>().GetPropertyValue(x => x.Phone) },
                    { "BankName", profile.BankName },
                    { "Swift", profile.SWIFT },
                    { "Iban", profile.IBAN },
                    { "PaymentEssentialElements", profile.PaymentEssentialElements },
                    { "ChiefNameInNominative", profile.ChiefNameInNominative },
                };
        }

        private PrintData GetBranchOfficeOrganizationUnitData(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                {
                    { "PostalAddress", branchOfficeOrganizationUnit.PostalAddress },
                    { "PhoneNumber", branchOfficeOrganizationUnit.PhoneNumber },
                    { "PaymentEssentialElements", branchOfficeOrganizationUnit.PaymentEssentialElements },
                    { "ChiefNameInNominative", branchOfficeOrganizationUnit.ChiefNameInNominative },
                };
        }
    }
}