﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Russia.Generic
{
    public static class OrderPrintFormDataExtractorSpecs
    {
        #region Test Data
        const long OrderId = 1;
        const long LegalPersonProfileId = 2;
        const long FirmAddressId = 3;

        static readonly FirmContact FirmContact = new FirmContact { ContactType = FirmAddressContactType.Phone };
        static readonly IDictionary<long, IEnumerable<FirmContact>> FirmContacts = new Dictionary<long, IEnumerable<FirmContact>> { { FirmAddressId, new[] { FirmContact } } };
        static readonly BargainType BargainType = new BargainType();
        static readonly BranchOffice BranchOffice = new BranchOffice { BargainType = BargainType };
        static readonly BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit = new BranchOfficeOrganizationUnit { BranchOffice = BranchOffice, IsPrimaryForRegionalSales = true, IsActive = true };
        static readonly Bill Bill = new Bill();
        static readonly ChileBranchOfficeOrganizationUnitPart BranchOfficeOrganizationUnitPart = new ChileBranchOfficeOrganizationUnitPart();
        static readonly LegalPerson LegalPerson = new LegalPerson();
        static readonly LegalPersonProfile LegalPersonProfile = new LegalPersonProfile();
        static readonly ChileLegalPersonProfilePart LegalPersonProfilePart = new ChileLegalPersonProfilePart();
        static readonly Bank Bank = new Bank();
        static readonly FirmAddress FirmAddress = new FirmAddress { Id = FirmAddressId };
        static readonly Platform.Model.Entities.Erm.Platform Platform = new Platform.Model.Entities.Erm.Platform();
        static readonly Position Position = new Position { Platform = Platform };
        static readonly PricePosition PricePosition = new PricePosition { Position = Position };
        static readonly OrganizationUnit OrganizationUnit = new OrganizationUnit { BranchOfficeOrganizationUnits = new[] { BranchOfficeOrganizationUnit } };
        static readonly Firm Firm = new Firm { FirmAddresses = new[] { FirmAddress } };
        static readonly Bargain Bargain = new Bargain();
        static readonly Order Order = new Order { BranchOfficeOrganizationUnit = BranchOfficeOrganizationUnit, DestOrganizationUnit = OrganizationUnit, Firm = Firm, Bargain = Bargain, SourceOrganizationUnit = OrganizationUnit, LegalPerson = LegalPerson, BeginDistributionDate = DateTime.Now, ReleaseCountPlan = 1 };
        static readonly OrderPosition OrderPosition = new OrderPosition { IsActive = true, PricePosition = PricePosition, Order = Order, Amount = 1 };
        
        #endregion

        private static IFormatterFactory CreateFormatterFactory()
        {
            var mock = Mock.Of<IFormatterFactory>();
            Mock.Get(mock)
                .Setup(x => x.Create(Moq.It.IsAny<Type>(), Moq.It.IsAny<FormatType>(), Moq.It.IsAny<int>()))
                .Returns(new StubFormatter());
            return mock;
        }

        private static ISecurityServiceUserIdentifier CreateUserIdentifierService()
        {
            var mock = Mock.Of<ISecurityServiceUserIdentifier>();
            Mock.Get(mock)
                .Setup(x => x.GetUserInfo(Moq.It.IsAny<long?>()))
                .Returns(new NullUserIdentity());
            return mock;
        }

        private static IQueryable<T> Query<T>(params T[] args)
        {
            return args.AsQueryable();
        }

        [Tags("BL", "Print", "Order", "Russia")]
        [Subject(typeof(IOrderPrintFormDataExtractor))]
        public abstract class WhenRequestingPrintData
        {
            protected static IOrderPrintFormDataExtractor DataExtractor;
            protected static PrintData Result;

            Establish context = () =>
            {
                DataExtractor = new OrderPrintFormDataExtractor(CreateFormatterFactory(), CreateUserIdentifierService());
            };

            Because of = () =>
            {
                Result = PrintData.Concat(DataExtractor.GetBranchOffice(Query(BranchOffice)),
                                          DataExtractor.GetOrder(Query(Order)),
                                          DataExtractor.GetOrderPositions(Query(Order), Query(OrderPosition)),
                                          DataExtractor.GetPaymentSchedule(Query(Bill)),
                                          DataExtractor.GetUngrouppedFields(Query(Order), BranchOfficeOrganizationUnit, LegalPerson, LegalPersonProfile, TemplateCode.OrderWithoutVatWithoutDiscount),
                                          DataExtractor.GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit),
                                          DataExtractor.GetLegalPersonProfile(LegalPersonProfile));
            };
        }

        public class WhenRequestingPrintDataWithoutVatWithoutDiscount : WhenRequestingPrintData
        {
            static readonly string[] SchedulePaymentsFieldSet =
                {
                    "PayablePlan",
                    "PaymentDatePlan",
                };

            static readonly string[] OrderPositionsFieldSet =
                {
                    "Amount",
                    "BeginDistributionDate",
                    "ElectronicMediaParagraph",
                    "FirmName",
                    "Name",
                    "PayablePlanWithoutVat",
                    "PricePerUnit",
                    "ReleaseCountPlan",
                };

            static readonly string[] RootFieldSet = 
                {
                    "AdvMatherialsDeadline",
                    "BeginContractParagraph",
                    "BranchOffice.Inn",
                    "BranchOffice.LegalAddress",
                    "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                    "BranchOfficeOrganizationUnit.Kpp",
                    "BranchOfficeOrganizationUnit.PaymentEssentialElements",
                    "BranchOfficeOrganizationUnit.ShortLegalName",
                    "ClientRequisitesParagraph",
                    "ElectronicMedia",
                    "Order.Number",
                    "Order.SignupDate",
                    "Order.PayablePlan",
                    "Order.PayablePlanWithoutVat",
                    "Order.OwnerName",
                    "Profile.ChiefNameInNominative",
                    "Profile.EmailForAccountingDocuments",
                    "RelatedBargainInfo",
                    "SourceElectronicMedia",
                    "TechnicalTerminationParagraph",
                };

            It should_contain_certain_data_set_for_root = () => Result.Select(pair => pair.Key).Should().Contain(RootFieldSet);
            It should_contain_certain_data_set_for_payments_shedule = () => Result.GetTable("SchedulePayments").First().Select(pair => pair.Key).Should().Contain(SchedulePaymentsFieldSet);
            It should_contain_certain_data_set_for_order_positions = () => Result.GetTable("OrderPositions").First().Select(pair => pair.Key).Should().Contain(OrderPositionsFieldSet);
        }

        public class WhenRequestingPrintDataWithVatWithoutDiscount : WhenRequestingPrintData
        {
            static readonly string[] SchedulePaymentsFieldSet =
                {
                    "PayablePlan",
                    "PaymentDatePlan",
                };

            static readonly string[] OrderPositionsFieldSet =
                {
                    "Amount",
                    "BeginDistributionDate",
                    "ElectronicMediaParagraph",
                    "FirmName",
                    "Name",
                    "PayablePlan",
                    "PayablePlanWithoutVat",
                    "PricePerUnit",
                    "ReleaseCountPlan",
                    "VatSum",
                };

            static readonly string[] RootFieldSet = 
                {
                    "AdvMatherialsDeadline",
                    "BeginContractParagraph",
                    "BranchOffice.Inn",
                    "BranchOffice.LegalAddress",
                    "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                    "BranchOfficeOrganizationUnit.Kpp",
                    "BranchOfficeOrganizationUnit.PaymentEssentialElements",
                    "BranchOfficeOrganizationUnit.ShortLegalName",
                    "ClientRequisitesParagraph",
                    "ElectronicMedia",
                    "Order.Number",
                    "Order.SignupDate",
                    "Order.PayablePlan",
                    "Order.PayablePlanWithoutVat",
                    "Order.VatPlan",
                    "Order.VatRatio",
                    "Order.VatSum",
                    "Order.OwnerName",
                    "Profile.ChiefNameInNominative",
                    "Profile.EmailForAccountingDocuments",
                    "RelatedBargainInfo",
                    "SourceElectronicMedia",
                    "TechnicalTerminationParagraph",
                };

            It should_contain_certain_data_set_for_root = () => Result.Select(pair => pair.Key).Should().Contain(RootFieldSet);
            It should_contain_certain_data_set_for_payments_shedule = () => Result.GetTable("SchedulePayments").First().Select(pair => pair.Key).Should().Contain(SchedulePaymentsFieldSet);
            It should_contain_certain_data_set_for_order_positions = () => Result.GetTable("OrderPositions").First().Select(pair => pair.Key).Should().Contain(OrderPositionsFieldSet);
        }

        public class WhenRequestingPrintDataWithoutVatWithDiscount : WhenRequestingPrintData
        {
            static readonly string[] SchedulePaymentsFieldSet =
                {
                    "PayablePlan",
                    "PaymentDatePlan",
                };

            static readonly string[] OrderPositionsFieldSet =
                {
                    "Amount",
                    "BeginDistributionDate",
                    "DiscountPercent",
                    "ElectronicMediaParagraph",
                    "FirmName",
                    "Name",
                    "PayablePlanWithoutVat",
                    "PriceForMonthWithDiscount",
                    "PricePerUnit",
                    "ReleaseCountPlan",
                };

            static readonly string[] RootFieldSet = 
                {
                    "AdvMatherialsDeadline",
                    "BeginContractParagraph",
                    "BranchOffice.Inn",
                    "BranchOffice.LegalAddress",
                    "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                    "BranchOfficeOrganizationUnit.Kpp",
                    "BranchOfficeOrganizationUnit.PaymentEssentialElements",
                    "BranchOfficeOrganizationUnit.ShortLegalName",
                    "BranchOfficeOrganizationUnit.ActualAddress",
                    "BranchOfficeOrganizationUnit.Email",
                    "ClientRequisitesParagraph",
                    "DiscountSum",
                    "ElectronicMedia",
                    "Order.Number",
                    "Order.SignupDate",
                    "Order.PayablePlan",
                    "Order.PayablePlanWithoutVat",
                    "Order.OwnerName",
                    "PriceWithoutDiscount",
                    "Profile.ChiefNameInNominative",
                    "Profile.EmailForAccountingDocuments",
                    "RelatedBargainInfo",
                    "SourceElectronicMedia",
                    "TechnicalTerminationParagraph",
                };

            It should_contain_certain_data_set_for_root = () => Result.Select(pair => pair.Key).Should().Contain(RootFieldSet);
            It should_contain_certain_data_set_for_payments_shedule = () => Result.GetTable("SchedulePayments").First().Select(pair => pair.Key).Should().Contain(SchedulePaymentsFieldSet);
            It should_contain_certain_data_set_for_order_positions = () => Result.GetTable("OrderPositions").First().Select(pair => pair.Key).Should().Contain(OrderPositionsFieldSet);
        }

        public class WhenRequestingPrintDataWithVatWithDiscount : WhenRequestingPrintData
        {
            static readonly string[] SchedulePaymentsFieldSet =
                {
                    "PayablePlan",
                    "PaymentDatePlan",
                };

            static readonly string[] OrderPositionsFieldSet =
                {
                    "Amount",
                    "BeginDistributionDate",
                    "DiscountPercent",
                    "ElectronicMediaParagraph",
                    "FirmName",
                    "Name",
                    "PayablePlan",
                    "PayablePlanWithoutVat",
                    "PriceForMonthWithDiscount",
                    "PricePerUnit",
                    "ReleaseCountPlan",
                    "VatSum",
                };

            static readonly string[] RootFieldSet = 
                {
                    "AdvMatherialsDeadline",
                    "BeginContractParagraph",
                    "BranchOffice.Inn",
                    "BranchOffice.LegalAddress",
                    "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                    "BranchOfficeOrganizationUnit.Kpp",
                    "BranchOfficeOrganizationUnit.PaymentEssentialElements",
                    "BranchOfficeOrganizationUnit.ShortLegalName",
                    "ClientRequisitesParagraph",
                    "DiscountSum",
                    "ElectronicMedia",
                    "Order.Number",
                    "Order.SignupDate",
                    "Order.PayablePlan",
                    "Order.PayablePlanWithoutVat",
                    "Order.VatSum",
                    "Order.OwnerName",
                    "PriceWithoutDiscount",
                    "Profile.ChiefNameInNominative",
                    "Profile.EmailForAccountingDocuments",
                    "RelatedBargainInfo",
                    "SourceElectronicMedia",
                    "TechnicalTerminationParagraph",
                };

            It should_contain_certain_data_set_for_root = () => Result.Select(pair => pair.Key).Should().Contain(RootFieldSet);
            It should_contain_certain_data_set_for_payments_shedule = () => Result.GetTable("SchedulePayments").First().Select(pair => pair.Key).Should().Contain(SchedulePaymentsFieldSet);
            It should_contain_certain_data_set_for_order_positions = () => Result.GetTable("OrderPositions").First().Select(pair => pair.Key).Should().Contain(OrderPositionsFieldSet);
        }

        private sealed class StubFormatter : IFormatter
        {
            public string Format(object data)
            {
                return data.ToString();
            }
        }
    }
}
