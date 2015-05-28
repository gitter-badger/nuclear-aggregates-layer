using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Operations.Generic.Modify.DomainEntityObtainers
{
    public class LegalPersonObtainerSpecs
    {
        public abstract class LegalPersonObtainerContext
        {
            protected static IFinder Finder;
            protected static LegalPersonDomainEntityDto DomainEntityDto;

            protected static LegalPersonObtainer LegalPersonObtainer;

            protected static LegalPerson LegalPerson;

            Establish context = () =>
                {
                    LegalPerson = new LegalPerson();
                    DomainEntityDto = new LegalPersonDomainEntityDto
                        {
                            // Задаем эти свойства, чтобы тесты не падали по NullReferenceException
                            OwnerRef = new EntityReference(1, null),
                            ClientRef = new EntityReference(1, null)
                        };

                    Finder = Mock.Of<IFinder>();

                    LegalPersonObtainer = new LegalPersonObtainer(Finder);
                };
        }

        public class LegalPersonObtainerResultContext : LegalPersonObtainerContext
        {
            protected static LegalPerson Result;

            Because of = () => { Result = LegalPersonObtainer.ObtainBusinessModelEntity(DomainEntityDto); };
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonObtainer))]
        public class When_legal_person_does_not_exist_in_persistance : LegalPersonObtainerResultContext
        {
            Establish context = () =>
                {
                    DomainEntityDto.Id = 0; // Признак того, что создаем новую сущность
                };

            It should_return_active_legal_person = () => Result.IsActive.Should().BeTrue();
            It should_return_not_deleted_legal_person = () => Result.IsDeleted.Should().BeFalse();
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonObtainer))]
        public class When_legal_person_exists_in_persistance : LegalPersonObtainerResultContext
        {
            private const long CurrentCreatedBy = 345;
            private const long CurrentModifiedBy = 456;
            private static readonly EntityReference TestCreatedBy = new EntityReference(CurrentCreatedBy + 1, "TestCreatedBy");
            private static readonly EntityReference TestModifiedBy = new EntityReference(CurrentModifiedBy + 1, "TestModifieddBy");
            private const bool TestIsActive = true;
            private const bool TestIsDeleted = true;
            private static readonly DateTime CurrentCreatedOn = DateTime.UtcNow;
            private static readonly DateTime CurrentModifiedOn = DateTime.UtcNow;
            private static readonly DateTime TestCreatedOn = CurrentCreatedOn.AddMonths(1);
            private static readonly DateTime TestModifiedOn = CurrentModifiedOn.AddMonths(2);

            private Establish context = () =>
                {
                    Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<FindSpecification<LegalPerson>>()).One()).Returns(LegalPerson);

                    DomainEntityDto.Id = 1; // Id != 0 => сущность создадим не на new, а возьмем из хранилища

                    DomainEntityDto.CreatedByRef = TestCreatedBy;
                    DomainEntityDto.ModifiedByRef = TestModifiedBy;
                    DomainEntityDto.IsActive = TestIsActive;
                    DomainEntityDto.IsDeleted = TestIsDeleted;
                    DomainEntityDto.ModifiedOn = TestModifiedOn;
                    DomainEntityDto.CreatedOn = TestCreatedOn;

                    LegalPerson.CreatedBy = CurrentCreatedBy;
                    LegalPerson.ModifiedBy = CurrentModifiedBy;
                    LegalPerson.IsActive = !TestIsActive;
                    LegalPerson.IsDeleted = !TestIsDeleted;
                    LegalPerson.CreatedOn = CurrentCreatedOn;
                    LegalPerson.ModifiedOn = CurrentModifiedOn;
                };

            private It should_not_change_legal_person_IsActive_Property = () => Result.IsActive.Should().Be(!TestIsActive);
            private It should_not_change_legal_person_IsDeleted_Property = () => Result.IsActive.Should().Be(!TestIsDeleted);
            private It should_not_change_legal_person_CreatedBy_Property = () => Result.CreatedBy.Should().Be(CurrentCreatedBy);
            private It should_not_change_legal_person_ModifiedBy_Property = () => Result.ModifiedBy.Should().Be(CurrentModifiedBy);
            private It should_not_change_legal_person_CreatedOn_Property = () => Result.CreatedOn.Should().Be(CurrentCreatedOn);
            private It should_not_change_legal_person_ModifiedOn_Property = () => Result.ModifiedOn.Should().Be(CurrentModifiedOn);
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonObtainer))]
        public class When_obtain_legal_person : LegalPersonObtainerResultContext
        {
            private const string TestInn = "TestInn";
            private const string TestVat = "TestVat";
            private const string TestKpp = "TestKpp";
            private const string TestIc = "TestIc";
            private const string TestPassportSeries = "TestPassportSeries";
            private const string TestPassportNumber = "TestPassportNumber";
            private const string TestPassportIssuedBy = "TestPassportIssuedBy";
            private const string TestRegistrationAddress = "TestRegistrationAddress";
            private const string TestCardNumber = "TestCardNumber";
            private const string TestComment = "TestComment";
            private const string TestLegalAddress = "TestLegalAddress";
            private const string TestLegalName = "TestLegalName";
            private const string TestShortName = "TestShortName";
            private const LegalPersonType TestLegalPersonTypeEnum = LegalPersonType.Businessman;
            private static readonly byte[] TestTimestamp = { 1, 2, 3, 4, 5, 6, 7, 8 };
            private static readonly EntityReference TestClient = new EntityReference(123, "TestClient");
            private static readonly EntityReference TestOwner = new EntityReference(234, "TestOwner");

            private Establish context = () =>
                {
                    DomainEntityDto.Inn = TestInn;
                    DomainEntityDto.VAT = TestVat;
                    DomainEntityDto.Kpp = TestKpp;
                    DomainEntityDto.Ic = TestIc;
                    DomainEntityDto.PassportSeries = TestPassportSeries;
                    DomainEntityDto.PassportNumber = TestPassportNumber;
                    DomainEntityDto.PassportIssuedBy = TestPassportIssuedBy;
                    DomainEntityDto.RegistrationAddress = TestRegistrationAddress;
                    DomainEntityDto.CardNumber = TestCardNumber;
                    DomainEntityDto.Comment = TestComment;
                    DomainEntityDto.LegalAddress = TestLegalAddress;
                    DomainEntityDto.Timestamp = TestTimestamp;
                    DomainEntityDto.LegalName = TestLegalName;
                    DomainEntityDto.ShortName = TestShortName;
                    DomainEntityDto.LegalPersonTypeEnum = TestLegalPersonTypeEnum;
                    DomainEntityDto.OwnerRef = TestOwner;
                    DomainEntityDto.ClientRef = TestClient;
                };

            private It should_return_expected_legal_person_Inn = () => Result.Inn.Should().Be(TestInn);
            private It should_return_expected_legal_person_Vat = () => Result.VAT.Should().Be(TestVat);
            private It should_return_expected_legal_person_Kpp = () => Result.Kpp.Should().Be(TestKpp);
            private It should_return_expected_legal_person_Ic = () => Result.Ic.Should().Be(TestIc);
            private It should_return_expected_legal_person_PassportSeries = () => Result.PassportSeries.Should().Be(TestPassportSeries);
            private It should_return_expected_legal_person_PassportNumber = () => Result.PassportNumber.Should().Be(TestPassportNumber);
            private It should_return_expected_legal_person_PassportIssuedBy = () => Result.PassportIssuedBy.Should().Be(TestPassportIssuedBy);
            private It should_return_expected_legal_person_RegistrationAddress = () => Result.RegistrationAddress.Should().Be(TestRegistrationAddress);
            private It should_return_expected_legal_person_CardNumber = () => Result.CardNumber.Should().Be(TestCardNumber);
            private It should_return_expected_legal_person_Comment = () => Result.Comment.Should().Be(TestComment);
            private It should_return_expected_legal_person_LegalAddress = () => Result.LegalAddress.Should().Be(TestLegalAddress);
            private It should_return_expected_legal_person_LegalName = () => Result.LegalName.Should().Be(TestLegalName);
            private It should_return_expected_legal_person_ShortName = () => Result.ShortName.Should().Be(TestShortName);
            private It should_return_expected_legal_person_LegalPersonTypeEnum = () => Result.LegalPersonTypeEnum.Should().Be(TestLegalPersonTypeEnum);
            private It should_return_expected_legal_person_Timestamp = () => Result.Timestamp.Should().BeEquivalentTo(TestTimestamp);
            private It should_return_expected_legal_person_Owner = () => Result.OwnerCode.Should().Be(TestOwner.Id);
            private It should_return_expected_legal_person_Client = () => Result.ClientId.Should().Be(TestClient.Id);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}