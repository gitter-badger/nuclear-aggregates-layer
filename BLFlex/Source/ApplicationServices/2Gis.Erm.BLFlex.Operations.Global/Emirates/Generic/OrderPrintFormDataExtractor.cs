using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic
{
    public sealed class OrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;

        public OrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
        }

        public PrintData GetOrder(IQueryable<Order> query)
        {
            var order = _printOrderHelper.GetOrder(query);

            return new PrintData 
            { 
                { "Order", order }, 
            };
        }

        public PrintData GetBranchOffice(IQueryable<BranchOffice> query)
        {
            var branchOffice = query
                .Select(office => new
                {
                    office.Inn,
                    office.LegalAddress,
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        {"Inn", x.Inn},
                        {"LegalAddress", x.LegalAddress},
                    })
                .Single();

            return new PrintData
                {
                    { "BranchOffice", branchOffice },
                };
        }

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou)
        {
            var branchOfficeOrganizationUnit = new PrintData
                {
                    { "ShortLegalName", boou.ShortLegalName },
                    { "PositionInNominative", boou.PositionInNominative },
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "PostalAddress", boou.PostalAddress },
                    { "PhoneNumber", boou.PhoneNumber },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },

                };

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnit },
                };
        }

        public PrintData GetBargain(Bargain bargain)
        {
            if (bargain == null)
            {
                return new PrintData
                {
                    { "UseBargain", false },
                };
            }

            var bargainData = new PrintData
                {
                    { "Number", bargain.Number },
                    { "SignedOn", bargain.SignedOn },
                };

            return new PrintData
                {
                    { "UseBargain", true },
                    { "Bargain", bargainData },
                };
        }

        public PrintData GetLegalPerson(LegalPerson legalPerson)
        {
            var legalPersonData = new PrintData
                {
                    { "LegalName", legalPerson.LegalName },
                    { "Inn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                };

            return new PrintData
                {
                    { "LegalPerson", legalPersonData },
                };
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            var profile = new PrintData
                {
                    { "PositionInNominative", legalPersonProfile.PositionInNominative },
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "EmailForAccountingDocuments", legalPersonProfile.EmailForAccountingDocuments },
                    { "PostAddress", legalPersonProfile.PostAddress },
                    { "Phone", legalPersonProfile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone },
                    { "BankName", legalPersonProfile.BankName },
                    { "SWIFT", legalPersonProfile.SWIFT },
                    { "IBAN", legalPersonProfile.IBAN },
                    { "PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                };

            return new PrintData
                {
                    { "Profile", profile },
                };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery)
        {
            return _printOrderHelper.GetOrderPositions(orderQuery, orderPositionsQuery);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query)
        {
            var stuff = query
                .Select(order => new
                {
                    order.BeginDistributionDate,
                    order.DestOrganizationUnit.ElectronicMedia,
                    FirmName = order.Firm.Name,
                })
                .Single();

            return new PrintData
                {
                    { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMatherialsDeadline(stuff.BeginDistributionDate) },
                    { "ElectronicMedia", stuff.ElectronicMedia },
                    { "Firm.Name", stuff.FirmName },
                };
        }

        public PrintData GetCategories(IQueryable<Order> query)
        {
            return _printOrderHelper.GetCategories(query);
        }

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts)
        {
            return _printOrderHelper.GetFirmAddresses(query, contacts, CultureInfo.CreateSpecificCulture("en"));
        }
    }
}
