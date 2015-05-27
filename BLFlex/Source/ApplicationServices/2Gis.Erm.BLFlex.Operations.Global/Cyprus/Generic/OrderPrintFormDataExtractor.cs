using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;
using EnumResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.EnumResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic
{
    public sealed class OrderPrintFormDataExtractor : IOrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;
        private readonly IFormatter _longDateFormatter;

        public OrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            var profile = new PrintData
                {
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "EmailForAccountingDocuments", legalPersonProfile.EmailForAccountingDocuments },
                };

            return new PrintData
                {
                    { "Profile", profile },
                };
        }

        public PrintData GetOrder(IQueryable<Order> query)
        {
            var order = _printOrderHelper.GetOrder(query);

            return new PrintData 
            { 
                { "Order", order }, 
            };
        }

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts)
        {
            return _printOrderHelper.GetFirmAddresses(query, contacts, CultureInfo.CreateSpecificCulture("en"));
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
                        { "Inn", x.Inn },
                        { "LegalAddress", x.LegalAddress }
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
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },
                    { "ShortLegalName", boou.ShortLegalName },
                };

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnit },
                };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery)
        {
            return _printOrderHelper.GetOrderPositions(orderQuery, orderPositionsQuery);
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query, LegalPerson legalPerson, LegalPersonProfile profile)
        {
            var bargain = query
                .Select(x => x.Bargain)
                .Where(x => x != null)
                .Select(x => new { x.Number, x.CreatedOn })
                .SingleOrDefault();

            var categories = _printOrderHelper.GetCategories(query);

            var stuff = query
                .Select(order => new
                {
                    order.BeginDistributionDate,
                    order.SignupDate,
                    order.DestOrganizationUnit.ElectronicMedia,
                    SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                    FirmName = order.Firm.Name,
                    order.LegalPersonId,
                    order.PaymentMethod,
                })
                .Single();

            var parintData = new PrintData
                {
                    { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMaterialsDeadline(stuff.BeginDistributionDate, stuff.SignupDate) },
                    { "ClientRequisitesParagraph", GetClientRequisitesParagraph(legalPerson, profile) },
                    { "ElectronicMedia", stuff.ElectronicMedia },
                    { "Firm.Name", stuff.FirmName },
                                     {
                                         "PaymentMethod",
                                         stuff.PaymentMethod != PaymentMethod.Undefined
                                             ? stuff.PaymentMethod.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                                             : string.Empty
                                     },
                    { "RelatedBargainInfo", bargain != null ? GetRelatedBargainInfo(bargain.Number, bargain.CreatedOn) : null },
                };

            return PrintData.Concat(categories, parintData);
        }

        private string GetRelatedBargainInfo(string bargainNumber, DateTime createdOn)
        {
            return string.Format(BLCoreResources.RelatedToBargainInfoTemplate, bargainNumber, _longDateFormatter.Format(createdOn));
        }

        private static string GetClientRequisitesParagraph(LegalPerson legalPerson, LegalPersonProfile profile)
        {
            switch (legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.NaturalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.CyprusPrintOrderHandler_ClientRequisitesParagraph1,
                        legalPerson.LegalName,
                        legalPerson.PassportSeries,
                        legalPerson.PassportNumber,
                        legalPerson.PassportIssuedBy,
                        legalPerson.RegistrationAddress);
                case LegalPersonType.Businessman:
                case LegalPersonType.LegalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.CyprusPrintOrderHandler_ClientRequisitesParagraph2,
                        legalPerson.LegalName,
                        legalPerson.Inn,
                        legalPerson.VAT,
                        legalPerson.LegalAddress,
                        profile != null ? profile.IBAN : string.Empty,
                        profile != null ? profile.SWIFT : string.Empty,
                        profile != null ? profile.AccountNumber : string.Empty,
                        profile != null ? profile.BankName : string.Empty,
                        profile != null ? profile.PaymentEssentialElements : string.Empty);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
