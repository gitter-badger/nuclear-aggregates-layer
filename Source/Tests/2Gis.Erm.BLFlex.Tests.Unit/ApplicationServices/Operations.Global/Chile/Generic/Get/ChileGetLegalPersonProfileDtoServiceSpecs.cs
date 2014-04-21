using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonProfileDtoServiceSpecs
    {
        public abstract class ChileGetLegalPersonProfileDtoServiceContext
        {
            protected static ChileGetLegalPersonProfileDtoService ChileGetLegalPersonProfileDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel ReadModel;
            protected static IBankReadModel BankReadModel;
            protected static ISecureFinder SecureFinder;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ReadModel = Mock.Of<ILegalPersonReadModel>();
                    BankReadModel = Mock.Of<IBankReadModel>();
                    UserContext = Mock.Of<IUserContext>();
                    SecureFinder = Mock.Of<ISecureFinder>();

                    ChileGetLegalPersonProfileDtoService = new ChileGetLegalPersonProfileDtoService(UserContext, SecureFinder, ReadModel, BankReadModel);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_existing_entity : ChileGetLegalPersonProfileDtoServiceContext
        {
            protected static ChileLegalPersonProfileDomainEntityDto Dto;
            protected static LegalPersonProfile LegalPersonProfile;
            protected static ChileLegalPersonProfilePart ChileLegalPersonProfilePart;
            protected static Bank TestBank;


            const long TestBankId = 2;
            const string TestBankName = "TestBankName";
            const AccountType TestAccountType = AccountType.SavingsAccount;
            const string TestRepresentativeRut = "TestRepresentativeRut";
            static readonly DateTime TestRepresentativeAuthorityDocumentIssuedOn = DateTime.UtcNow;
            const string TestRepresentativeAuthorityDocumentIssuedBy = "TestRepresentativeAuthorityDocumentIssuedBy";

            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища
                    Dto = new ChileLegalPersonProfileDomainEntityDto();
                    TestBank = new Bank { Id = TestBankId, Name = TestBankName };

                    ChileLegalPersonProfilePart = new ChileLegalPersonProfilePart
                        {
                            BankId = TestBankId,
                            AccountType = TestAccountType,
                            RepresentativeRut = TestRepresentativeRut,
                            RepresentativeAuthorityDocumentIssuedOn = TestRepresentativeAuthorityDocumentIssuedOn,
                            RepresentativeAuthorityDocumentIssuedBy = TestRepresentativeAuthorityDocumentIssuedBy
                        };

                    LegalPersonProfile = new LegalPersonProfile { Parts = new[] { ChileLegalPersonProfilePart } };

                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonProfileDto<ChileLegalPersonProfileDomainEntityDto>(entityId)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonProfile(EntityId)).Returns(LegalPersonProfile);
                    Mock.Get(BankReadModel).Setup(x => x.GetBank(Moq.It.IsAny<long>())).Returns(TestBank);
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
            static IUserIdentity _userIdentity;
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            static LegalPerson ParenLegalPerson;
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;

            Establish context = () =>
                {
                    EntityId = 0; // C Id = 0 мы создадим новую Dto
                    ParentEntityId = 1;
                    ParentEntityName = EntityName.LegalPerson;

                    ParenLegalPerson = new LegalPerson
                        {
                            Id = ParentEntityId.Value,
                            LegalName = TestLegalPersonName,
                        };

                    _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                    Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
                    Mock.Get(SecureFinder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<LegalPerson>>())).Returns(Q(ParenLegalPerson));
                };

            Because of = () =>
            {
                Result = ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
            };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonProfileDomainEntityDto>();
            It should_have_default_PaymentMethod = () => ((ChileLegalPersonProfileDomainEntityDto)Result).PaymentMethod.Should().Be(DefaultPaymentMethod);

            It should_have_default_DocumentsDeliveryMethod =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).DocumentsDeliveryMethod.Should().Be(DefaultDocumentsDeliveryMethod);

            It should_have_expected_LegalPersonId = () => ((ChileLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Id.Should().Be(ParenLegalPerson.Id);

            It should_have_expected_LegalPersonName =
                () => ((ChileLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Name.Should().Be(ParenLegalPerson.LegalName);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_but_parentId_is_not_set : ChileGetLegalPersonProfileDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;
            static Exception catchedException;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                ParentEntityId = null;
                ParentEntityName = EntityName.LegalPerson;

                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            Because of = () => catchedException = Catch.Exception(() => ChileGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentNullException = () => catchedException.Should().BeOfType<ArgumentNullException>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_and_parent_entity_is_not_a_LegalPerson : ChileGetLegalPersonProfileDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            const PaymentMethod DefaultPaymentMethod = PaymentMethod.BankTransaction;
            const DocumentsDeliveryMethod DefaultDocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined;
            static Exception catchedException;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                ParentEntityId = 1;
                ParentEntityName = EntityName.Order;

                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
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
