using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonProfileDtoServiceSpecs
    {
        public abstract class UkraineGetLegalPersonProfileDtoServiceContext
        {
            protected static UkraineGetLegalPersonProfileDtoService UkraineGetLegalPersonProfileDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel LegalPersonReadModel;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static IEntityType ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    LegalPersonReadModel = Mock.Of<ILegalPersonReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());

                    UkraineGetLegalPersonProfileDtoService = new UkraineGetLegalPersonProfileDtoService(UserContext, LegalPersonReadModel);
                };
        }

        [Obsolete("Test must be actualized")]
        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_existing_entity : UkraineGetLegalPersonProfileDtoServiceContext
        {
            protected static LegalPersonProfile LegalPersonProfile;
            protected static UkraineLegalPersonProfilePart UkraineLegalPersonProfilePart;

            const string TestMfo = "TestMfo";
            const TaxationType TestTaxationType = TaxationType.WithoutVat;
            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища

                    UkraineLegalPersonProfilePart = new UkraineLegalPersonProfilePart { Mfo = TestMfo };
                    LegalPersonProfile = new LegalPersonProfile { Parts = new[] { UkraineLegalPersonProfilePart }, Timestamp = new byte[0] };  // Timestamp != null показывает, что LegalPerson не новая сущность

                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPersonProfile(EntityId)).Returns(LegalPersonProfile);
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

                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPersonName(ParentEntityId.Value)).Returns(TestLegalPersonName);
                };

            Because of = () =>
            {
                Result = UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
            };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonProfileDomainEntityDto>();
            It should_have_default_PaymentMethod = () => ((UkraineLegalPersonProfileDomainEntityDto)Result).PaymentMethod.Should().Be(DefaultPaymentMethod);

            It should_have_default_DocumentsDeliveryMethod =
                () => ((UkraineLegalPersonProfileDomainEntityDto)Result).DocumentsDeliveryMethod.Should().Be(DefaultDocumentsDeliveryMethod);

            It should_have_expected_LegalPersonId = () => ((UkraineLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Id.Should().Be(ParentLegalPerson.Id);

            It should_have_expected_LegalPersonName =
                () => ((UkraineLegalPersonProfileDomainEntityDto)Result).LegalPersonRef.Name.Should().Be(ParentLegalPerson.LegalName);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_but_parentId_is_not_set : UkraineGetLegalPersonProfileDtoServiceContext
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

            Because of = () => catchedException = Catch.Exception(() => UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentNullException = () => catchedException.Should().BeOfType<ArgumentNullException>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonProfileDtoService))]
        class When_requested_new_entity_and_parent_entity_is_not_a_LegalPerson : UkraineGetLegalPersonProfileDtoServiceContext
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

            Because of = () => catchedException = Catch.Exception(() => UkraineGetLegalPersonProfileDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo));

            It should_throw_ArgumentException = () => catchedException.Should().BeOfType<ArgumentException>();
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
