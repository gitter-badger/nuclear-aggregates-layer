using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonProfileDtoServiceSpecs
    {
        public abstract class UkraineGetLegalPersonProfileDtoServiceContext
        {
            protected static UkraineGetLegalPersonProfileDtoService UkraineGetLegalPersonProfileDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel ReadModel;
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
                    UserContext = Mock.Of<IUserContext>();
                    SecureFinder = Mock.Of<ISecureFinder>();

                    UkraineGetLegalPersonProfileDtoService = new UkraineGetLegalPersonProfileDtoService(UserContext, SecureFinder, ReadModel);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_existing_entity : UkraineGetLegalPersonProfileDtoServiceContext
        {
            protected static UkraineLegalPersonProfileDomainEntityDto Dto;
            protected static LegalPersonProfile LegalPersonProfile;
            protected static UkraineLegalPersonProfilePart UkraineLegalPersonProfilePart;

            const string TestMfo = "TestMfo";
            const TaxationType TestTaxationType = TaxationType.WithoutVat;
            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища
                    Dto = new UkraineLegalPersonProfileDomainEntityDto();

                    UkraineLegalPersonProfilePart = new UkraineLegalPersonProfilePart { Mfo = TestMfo};
                    LegalPersonProfile = new LegalPersonProfile { Parts = new[] { UkraineLegalPersonProfilePart } };

                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonProfileDto<UkraineLegalPersonProfileDomainEntityDto>(entityId)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonProfile(EntityId)).Returns(LegalPersonProfile);
                };

            Because of = () =>
            {
                Result = UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
            };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonProfileDomainEntityDto>();

            It should_have_expected_Mfo = () => ((UkraineLegalPersonProfileDomainEntityDto)Result).Mfo.Should().Be(TestMfo);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_new_entity : UkraineGetLegalPersonProfileDtoServiceContext
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
                Result = UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
            };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonProfileDomainEntityDto>();
            It should_have_default_PaymentMethod = () => ((UkraineLegalPersonProfileDomainEntityDto)Result).PaymentMethod.Should().Be(DefaultPaymentMethod);

            It should_have_default_DocumentsDeliveryMethod =
                () => ((UkraineLegalPersonProfileDomainEntityDto)Result).DocumentsDeliveryMethod.Should().Be(DefaultDocumentsDeliveryMethod);

            It should_have_expected_LegalPersonId = () => ((UkraineLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Id.Should().Be(ParenLegalPerson.Id);

            It should_have_expected_LegalPersonName =
                () => ((UkraineLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Name.Should().Be(ParenLegalPerson.LegalName);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_but_parentId_is_not_set : UkraineGetLegalPersonProfileDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            static LegalPerson ParenLegalPerson;
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

            Because of = () => catchedException = Catch.Exception(() => UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentNullException = () => catchedException.Should().BeOfType<ArgumentNullException>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_and_parent_entity_is_not_a_LegalPerson : UkraineGetLegalPersonProfileDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;
            const string TestLegalPersonName = "TestLegalPersonName";
            static LegalPerson ParenLegalPerson;
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

            Because of = () => catchedException = Catch.Exception(() => UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentException = () => catchedException.Should().BeOfType<ArgumentException>();
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
