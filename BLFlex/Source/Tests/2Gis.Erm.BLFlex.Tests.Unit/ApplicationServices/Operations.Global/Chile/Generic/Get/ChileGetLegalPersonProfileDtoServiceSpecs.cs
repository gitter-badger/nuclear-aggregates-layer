using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonProfileDtoServiceSpecs
    {
        public abstract class ChileGetLegalPersonProfileDtoServiceContext
        {
            protected static ChileGetLegalPersonProfileDtoService ChileGetLegalPersonProfileDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel LegalPersonReadModel;
            protected static IBankReadModel BankReadModel;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static IEntityType ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    LegalPersonReadModel = Mock.Of<ILegalPersonReadModel>();
                    BankReadModel = Mock.Of<IBankReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());

                    ChileGetLegalPersonProfileDtoService = new ChileGetLegalPersonProfileDtoService(UserContext, LegalPersonReadModel, BankReadModel);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_existing_entity : ChileGetLegalPersonProfileDtoServiceContext
        {
            protected static LegalPersonProfile LegalPersonProfile;
            protected static ChileLegalPersonProfilePart ChileLegalPersonProfilePart;

            const long TestBankId = 2;
            const string TestBankName = "TestBankName";
            const AccountType TestAccountType = AccountType.SavingsAccount;
            const string TestRepresentativeRut = "TestRepresentativeRut";
            const string TestRepresentativeAuthorityDocumentIssuedBy = "TestRepresentativeAuthorityDocumentIssuedBy";
            static readonly DateTime TestRepresentativeAuthorityDocumentIssuedOn = DateTime.UtcNow;

            Establish context = () =>
                {
                    EntityId = 1; // C Id != 0 мы получим Dto из хранилища

                    ChileLegalPersonProfilePart = new ChileLegalPersonProfilePart
                        {
                            BankId = TestBankId,
                            AccountType = TestAccountType,
                            RepresentativeRut = TestRepresentativeRut,
                            RepresentativeAuthorityDocumentIssuedOn = TestRepresentativeAuthorityDocumentIssuedOn,
                            RepresentativeAuthorityDocumentIssuedBy = TestRepresentativeAuthorityDocumentIssuedBy
                        };

                    LegalPersonProfile = new LegalPersonProfile { Parts = new[] { ChileLegalPersonProfilePart }, Timestamp = new byte[0] }; // Timestamp != null указывает, что объект не новый

                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPersonProfile(EntityId)).Returns(LegalPersonProfile);
                    Mock.Get(BankReadModel).Setup(x => x.GetBankName(TestBankId)).Returns(TestBankName);
                };

            Because of = () =>
                {
                    Result = ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonProfileDomainEntityDto>();

            It should_have_expected_BankId = () => ((ChileLegalPersonProfileDomainEntityDto)Result).BankRef.Id.Should().Be(TestBankId);
            It should_have_expected_BankName = () => ((ChileLegalPersonProfileDomainEntityDto)Result).BankRef.Name.Should().Be(TestBankName);
            It should_have_expected_AccountType = () => ((ChileLegalPersonProfileDomainEntityDto)Result).AccountType.Should().Be(TestAccountType);

            It should_have_expected_RepresentativeRut =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).RepresentativeRut.Should().Be(TestRepresentativeRut);

            It should_have_expected_RepresentativeDocumentIssuedOn =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).RepresentativeDocumentIssuedOn.Should().Be(TestRepresentativeAuthorityDocumentIssuedOn);

            It should_have_expected_RepresentativeAuthorityDocumentIssuedBy =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).RepresentativeDocumentIssuedBy.Should().Be(TestRepresentativeAuthorityDocumentIssuedBy);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_new_entity : ChileGetLegalPersonProfileDtoServiceContext
        {
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;
            static LegalPerson ParentLegalPerson;

            Establish context = () =>
                {
                    EntityId = 0; // C Id = 0 мы создадим новую Dto
                    ParentEntityId = 1;
                    ParentEntityName = EntityType.Instance.LegalPerson();

                    ParentLegalPerson = new LegalPerson
                        {
                            Id = ParentEntityId.Value,
                            LegalName = TestLegalPersonName,
                        };

                    var userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                    Mock.Get(UserContext).Setup(x => x.Identity).Returns(userIdentity);
                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPersonName(ParentEntityId.Value)).Returns(TestLegalPersonName);
                };

            Because of = () =>
            {
                Result = ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
            };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonProfileDomainEntityDto>();
            It should_have_default_PaymentMethod = () => ((ChileLegalPersonProfileDomainEntityDto)Result).PaymentMethod.Should().Be(DefaultPaymentMethod);

            It should_have_default_DocumentsDeliveryMethod =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).DocumentsDeliveryMethod.Should().Be(DefaultDocumentsDeliveryMethod);

            It should_have_expected_LegalPersonId = () => ((ChileLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Id.Should().Be(ParentLegalPerson.Id);

            It should_have_expected_LegalPersonName =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Name.Should().Be(ParentLegalPerson.LegalName);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_but_parentId_is_not_set : ChileGetLegalPersonProfileDtoServiceContext
        {
            const string TestLegalPersonName = "TestLegalPersonName";
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;
            static Exception catchedException;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                ParentEntityId = null;
                ParentEntityName = EntityType.Instance.LegalPerson();
            };

            Because of = () => catchedException = Catch.Exception(() => ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentNullException = () => catchedException.Should().BeOfType<ArgumentNullException>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_and_parent_entity_is_not_a_LegalPerson : ChileGetLegalPersonProfileDtoServiceContext
        {
            const string TestLegalPersonName = "TestLegalPersonName";
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;
            static Exception catchedException;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                ParentEntityId = 1;
                ParentEntityName = EntityType.Instance.Order();
            };

            Because of = () => catchedException = Catch.Exception(() => ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentException = () => catchedException.Should().BeOfType<ArgumentException>();
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
