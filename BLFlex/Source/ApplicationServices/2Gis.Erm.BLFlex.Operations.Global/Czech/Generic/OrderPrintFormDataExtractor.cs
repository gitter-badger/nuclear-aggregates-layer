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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic
{
    public sealed class OrderPrintFormDataExtractor : IOrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;

        public OrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            var profile = new PrintData
                {
                    { "ChiefNameInGenitive", legalPersonProfile.ChiefNameInGenitive },
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "AccountNumber", legalPersonProfile.AccountNumber },
                    { "BankCode", legalPersonProfile.BankCode },
                    { "BankName", legalPersonProfile.BankName },
                    { "Registered", legalPersonProfile.Registered },
                    { "EmailForAccountingDocuments", legalPersonProfile.EmailForAccountingDocuments },
                };

            return new PrintData
                {
                    { "Profile", profile },
                    { "OperatesOnTheBasis", GetOperatesOnTheBasisString(legalPersonProfile) }
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
            return _printOrderHelper.GetFirmAddresses(query, contacts, CultureInfo.CurrentCulture);
        }

        public PrintData GetBranchOffice(IQueryable<BranchOffice> query)
        {
            var branchOffice = query
                .Select(office => new
                {
                    office.Ic,
                    office.Inn,
                    office.LegalAddress,
                    office.Name,
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Ic", x.Ic },
                        { "Inn", x.Inn },
                        { "LegalAddress", x.LegalAddress },
                        { "Name", x.Name },
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
                    { "ChiefNameInGenitive", boou.ChiefNameInGenitive },
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },
                    { "PositionInGenitive", boou.PositionInGenitive },
                    { "Registered", boou.Registered },
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

        public PrintData GetLegalPerson(LegalPerson legalPerson)
        {
            var type = legalPerson.LegalPersonTypeEnum;
            var legalPersonData = new PrintData
                {
                    { "Ic", legalPerson.Ic },
                    { "Inn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                    { "Prefix", GetPersonPrefix(type) },

                    { "UseInn", !string.IsNullOrWhiteSpace(legalPerson.Inn) },
                    { "UseLegalPerson", type == LegalPersonType.LegalPerson },
                    { "UseBusinessman", type == LegalPersonType.Businessman },
                };

            return new PrintData
                {
                    { "LegalPerson", legalPersonData },
                };
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query)
        {
            var categories = _printOrderHelper.GetCategories(query);

            var allTheFieldsThatHaveNoRelationToEverythingElse = query
                .Select(order => new
                {
                    order.DestOrganizationUnit.ElectronicMedia,
                    SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                    FirmName = order.Firm.Name,
                    Order = order,
                    TerminatedOrder = order.TechnicallyTerminatedOrder,
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "ElectronicMedia", x.ElectronicMedia },
                        { "SourceElectronicMedia", x.SourceElectronicMedia },
                        { "Firm.Name", x.FirmName },
                        { "TerminatedOrder", GetTerminatedOrder(x.TerminatedOrder) },
                        { "UseTechnicalTermination", x.TerminatedOrder != null },
                    })
                .Single();

            return PrintData.Concat(categories, allTheFieldsThatHaveNoRelationToEverythingElse);
        }

        public PrintData GetClient(LegalPerson legalPerson, LegalPersonProfile legalPersonProfile)
        {
            return new PrintData
                {
                    { "ClientLegalNamePrefix", GetClientLegalNamePrefixTemplate(legalPerson.LegalPersonTypeEnum) },
                };
        }

        private static string GetPersonPrefix(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLFlexResources.CzechPrintOrderHandler_PersonPrefixBusinessman
                : BLFlexResources.CzechPrintOrderHandler_PersonPrefixLegalPerson;
        }

        private static string GetClientLegalNamePrefixTemplate(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLFlexResources.CzechPrintOrderHandler_ClientLegalNamePrefixBusinessman
                : BLFlexResources.CzechPrintOrderHandler_ClientLegalNamePrefixLegalPerson;
        }

        private static string GetOperatesOnTheBasisString(LegalPersonProfile profile)
        {
            if (profile.OperatesOnTheBasisInGenitive != OperatesOnTheBasisType.Warranty)
            {
                return string.Empty;
            }

            if (profile.WarrantyBeginDate == null)
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                BLFlexResources.CzechPrintOrderHandler_OperatesOnTheBasisStringTemplate,
                profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                profile.WarrantyBeginDate.Value.ToShortDateString()); // FIXME {a.rechkalov, 12.03.2014}: неправильное форматирование даты
        }

        private PrintData GetTerminatedOrder(Order terminatedOrder)
        {
            return terminatedOrder == null
                       ? null
                       : new PrintData
                             {
                                 { "Number", terminatedOrder.Number },
                                 { "SignupDate", terminatedOrder.SignupDate },
                                 { "EndDistributionDateFact", terminatedOrder.EndDistributionDateFact },
                             };
        }
    }
}
