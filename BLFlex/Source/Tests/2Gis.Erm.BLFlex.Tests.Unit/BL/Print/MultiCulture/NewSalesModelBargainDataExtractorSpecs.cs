using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;
using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.BL.Print.MultiCulture
{
    public static class NewSalesModelBargainDataExtractorSpecs
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
        static readonly LegalPerson LegalPerson = new LegalPerson();
        static readonly LegalPersonProfile LegalPersonProfile = new LegalPersonProfile { OperatesOnTheBasisInGenitive = OperatesOnTheBasisType.Charter };
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

        private static IQueryable<T> Query<T>(params T[] args)
        {
            return args.AsQueryable();
        }

        [Tags("BL", "Print", "Bargain", "Russia")]
        [Subject(typeof(IBargainPrintFormDataExtractor))]
        public class WhenRequestingPrintData
        {
            protected static IBargainPrintFormDataExtractor DataExtractor;
            protected static PrintData Result;

            static readonly string[] RootFieldSet =
                {
                    "UseBusinessman",
                    "UseLegalPerson",
                    "UseNaturalPerson",
                    "Bargain.Number",
                    "Bargain.SignedOn",
                    "BranchOffice.Inn",
                    "BranchOffice.LegalAddress",
                    "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                    "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                    "BranchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive",
                    "BranchOfficeOrganizationUnit.Kpp",
                    "BranchOfficeOrganizationUnit.PaymentEssentialElements",
                    "BranchOfficeOrganizationUnit.PositionInGenitive",
                    "BranchOfficeOrganizationUnit.PositionInNominative",
                    "BranchOfficeOrganizationUnit.ShortLegalName",
                    "BranchOfficeOrganizationUnit.ActualAddress",
                    "BranchOfficeOrganizationUnit.Email",
                    "LegalPerson.Inn",
                    "LegalPerson.Kpp",
                    "LegalPerson.LegalAddress",
                    "LegalPerson.LegalName",
                    "LegalPerson.PassportIssuedBy",
                    "LegalPerson.PassportNumber",
                    "LegalPerson.PassportSeries",
                    "LegalPerson.RegistrationAddress",
                    "LegalPerson.ShortName",
                    "LegalPersonProfile.ChiefNameInNominative",
                    "LegalPersonProfile.PaymentEssentialElements",
                    "LegalPersonProfile.PositionInNominative",
                    "LegalPersonProfile.ChiefNameInGenitive",
                    "LegalPersonProfile.PositionInGenitive",
                    "OrganizationUnitName",
                    "LegalPersonProfile.DocumentName",
                    "LegalPersonProfile.CertificateNumber",
                    "LegalPersonProfile.CertificateDate",
                    "LegalPersonProfile.WarrantyNumber",
                    "LegalPersonProfile.WarrantyBeginDate",
                    "LegalPersonProfile.BargainNumber",
                    "LegalPersonProfile.BargainBeginDate",
                    "UseBargain", 
                    "UseCertificate", 
                    "UseCharter", 
                    "UseFoundingBargain", 
                    "UseRegistrationCertificate", 
                    "UseWarranty", 
                };

            Establish context = () =>
            {
                DataExtractor = new BargainPrintFormDataExtractor();
            };

            Because of = () =>
                {
                    Result = PrintData.Concat(DataExtractor.GetBargain(Query(Bargain)),
                                              DataExtractor.GetLegalPersonProfile(LegalPersonProfile),
                                              DataExtractor.GetLegalPerson(LegalPerson),
                                              DataExtractor.GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit),
                                              DataExtractor.GetBranchOffice(Query(BranchOffice)),
                                              DataExtractor.GetUngroupedFields(Query(Bargain)));
                };

            It should_contain_certain_data_set_for_root = () => Result.Select(pair => pair.Key).Should().Contain(RootFieldSet);
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
