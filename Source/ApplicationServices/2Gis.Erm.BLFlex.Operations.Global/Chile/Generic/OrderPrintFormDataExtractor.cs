﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;
using EnumResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.EnumResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic
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

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile, LegalPersonProfilePart legalPersonProfilePart, Bank bank)
        {
            var part = new PrintData
                {
                    { "AccountType", legalPersonProfilePart.AccountType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture) },
                    { "RepresentativeAuthorityDocumentIssuedBy", legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedBy },
                    { "RepresentativeAuthorityDocumentIssuedOn", legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedOn },
                    { "RepresentativeRut", legalPersonProfilePart.RepresentativeRut },
                    { "BankName", bank != null ? bank.Name : string.Empty },
                };

            var profile = new PrintData
                {
                    { "AccountNumber", legalPersonProfile.AccountNumber },
                    { "AdditionalPaymentElements", legalPersonProfile.AdditionalPaymentElements },
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "EmailForAccountingDocuments", legalPersonProfile.EmailForAccountingDocuments },
                    { "PositionInNominative", legalPersonProfile.PositionInNominative },
                };

            return new PrintData
                {
                    { "Profile", profile },
                    { "ProfilePart", part },
                };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> query)
        {
            return _printOrderHelper.GetOrderPositions(orderQuery, query);
        }

        public PrintData GetLegalPerson(LegalPerson legalPerson)
        {
            var legalPersonData = new PrintData
                {
                    { "Inn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                };

            return new PrintData
                {
                    { "LegalPerson", legalPersonData },
                };
        }

        public PrintData GetOrder(IQueryable<Order> query)
        {
            var order = _printOrderHelper.GetOrder(query);
            var orderExtension = _printOrderHelper.GetOrderExtension(query);

            return new PrintData { { "Order", PrintData.Concat(order, orderExtension) } };
        }

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts)
        {
            return _printOrderHelper.GetFirmAddresses(query, contacts);
        }

        public PrintData GetBranchOffice(IQueryable<BranchOffice> query)
        {
            var branchOffice = query
                .Select(office => new
                    {
                        office.Inn,
                        office.LegalAddress,
                        office.Name,
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
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

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou, BranchOfficeOrganizationUnitPart boouPart)
        {
            var branchOfficeOrganizationUnit = new PrintData
                {
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },
                    { "PositionInNominative", boou.PositionInNominative },
                };

            var branchOfficeOrganizationUnitPart = new PrintData
                {
                    { "RepresentativeRut", boouPart.RepresentativeRut },
                };

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnit },
                    { "BranchOfficeOrganizationUnitPart", branchOfficeOrganizationUnitPart },
                };
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query)
        {
            var categories = _printOrderHelper.GetCategories(query);

            var stuff = query
                .Select(order => new
                    {
                        order.BeginDistributionDate,
                        BargainNumber = order.Bargain.Number,
                        order.DestOrganizationUnit.ElectronicMedia,
                        SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                        FirmName = order.Firm.Name,
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "BargainNumber", x.BargainNumber ?? string.Empty },
                        { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMatherialsDeadline(x.BeginDistributionDate) },
                        { "ElectronicMedia", x.ElectronicMedia },
                        { "SourceElectronicMedia", x.SourceElectronicMedia },
                        { "Firm.Name", x.FirmName },
                    })
                .Single();

            return PrintData.Concat(categories, stuff);
        }
    }
}
