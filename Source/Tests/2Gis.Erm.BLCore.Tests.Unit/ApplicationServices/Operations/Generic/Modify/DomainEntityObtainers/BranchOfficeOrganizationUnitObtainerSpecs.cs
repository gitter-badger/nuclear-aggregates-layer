using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Operations.Generic.Modify.DomainEntityObtainers
{
    public class BranchOfficeOrganizationUnitObtainerSpecs
    {
        public abstract class BranchOfficeOrganizationUnitObtainerContext
        {
            protected static IFinder Finder;
            protected static IBusinessModelEntityObtainerFlex<BranchOfficeOrganizationUnit> FlexBehaviour;
            protected static BranchOfficeOrganizationUnitDomainEntityDto DomainEntityDto;

            protected static BranchOfficeOrganizationUnitObtainer BranchOfficeOrganizationUnitObtainer;

            protected static BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit;
            protected static EntityReference BranchOfficeRef;
            protected static EntityReference OrganizationUnitRef;

            Establish context = () =>
            {
                BranchOfficeOrganizationUnit = new BranchOfficeOrganizationUnit();
                BranchOfficeRef = new EntityReference();
                OrganizationUnitRef = new EntityReference();
                DomainEntityDto = new BranchOfficeOrganizationUnitDomainEntityDto
                {
                    BranchOfficeRef = BranchOfficeRef,
                    OrganizationUnitRef = OrganizationUnitRef
                };

                Finder = Mock.Of<IFinder>();
                FlexBehaviour = Mock.Of<IBusinessModelEntityObtainerFlex<BranchOfficeOrganizationUnit>>();

                BranchOfficeOrganizationUnitObtainer = new BranchOfficeOrganizationUnitObtainer(Finder, FlexBehaviour);
            };
        }

        public class BranchOfficeOrganizationUnitObtainerResultContext : BranchOfficeOrganizationUnitObtainerContext
        {
            protected static BranchOfficeOrganizationUnit Result;

            Because of = () => { Result = BranchOfficeOrganizationUnitObtainer.ObtainBusinessModelEntity(DomainEntityDto); };
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeOrganizationUnitObtainer))]
        public class When_create_existing_entity : BranchOfficeOrganizationUnitObtainerContext
        {
            protected static Exception catchedException;

            Establish context = () =>
            {
                BranchOfficeOrganizationUnit.Timestamp = new byte[0]; // not null

                Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOfficeOrganizationUnit>>())).Returns(Q(BranchOfficeOrganizationUnit));
            };

            Because of = () => catchedException = Catch.Exception(() => BranchOfficeOrganizationUnitObtainer.ObtainBusinessModelEntity(DomainEntityDto));

            It should_throws_business_logic_exception = () => catchedException.Should().BeOfType<BusinessLogicException>();
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeOrganizationUnitObtainer))]
        public class When_branch_office_not_exists_in_persistance : BranchOfficeOrganizationUnitObtainerResultContext
        {
            const long DOMAIN_ENTITY_DTO_ID = 1;

            Establish context = () =>
            {
                BranchOfficeRef.Id = 0;
                OrganizationUnitRef.Id = 0;

                DomainEntityDto.Id = DOMAIN_ENTITY_DTO_ID;
            };

            It should_return_active_boou = () => Result.IsActive.Should().BeTrue();
            It should_return_not_deleted_boou = () => Result.IsDeleted.Should().BeFalse();

            It should_return_boou_with_expected_id = () => Result.Id.Should().Be(DOMAIN_ENTITY_DTO_ID);
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeOrganizationUnitObtainer))]
        public class When_obtain_entity : BranchOfficeOrganizationUnitObtainerResultContext
        {
            const string SHORT_LEGAL_NAME = "SHORT_LEGAL_NAME";
            static readonly long? BRANCH_OFFICE_ID = 1;
            static readonly long? ORG_UNIT_ID = 2;
            const string PRONE_NUMBER = "PRONE_NUMBER";
            const string CHIEF_NAME_NOM = "CHIEF_NAME_NOM";
            const string CHIEF_NAME_GEN = "CHIEF_NAME_GEN";
            const string POSITION_NOM = "POSITION_NOM";
            const string POSITION_GEN = "POSITION_GEN";
            const string OPERTATES_GEN = "OPERTATES_GEN";
            const string REG_CERTIFICATE = "REG_CERTIFICATE";
            const string ACTUAL_ADDRESS = "ACTUAL_ADDRESS";
            const string EMAIL = "EMAIL";
            const string POST_ADDRESS = "POST_ADDRESS";
            const string PAYMENT = "PAYMENT";
            const string REGISTERED = "REGISTERED";
            const string KPP = "KPP";
            const string SYNC_1C_CODE = "SYNC_1C_CODE";
            static readonly byte[] TIMESTAMP = new byte[0];

            Establish context = () =>
            {
                DomainEntityDto.ShortLegalName = SHORT_LEGAL_NAME;
                BranchOfficeRef.Id = BRANCH_OFFICE_ID;
                OrganizationUnitRef.Id = ORG_UNIT_ID;
                DomainEntityDto.PhoneNumber = PRONE_NUMBER;
                DomainEntityDto.ChiefNameInNominative = CHIEF_NAME_NOM;
                DomainEntityDto.ChiefNameInGenitive = CHIEF_NAME_GEN;
                DomainEntityDto.PositionInNominative = POSITION_NOM;
                DomainEntityDto.PositionInGenitive = POSITION_GEN;
                DomainEntityDto.OperatesOnTheBasisInGenitive = OPERTATES_GEN;
                DomainEntityDto.RegistrationCertificate = REG_CERTIFICATE;
                DomainEntityDto.ActualAddress = ACTUAL_ADDRESS;
                DomainEntityDto.Email = EMAIL;
                DomainEntityDto.PostalAddress = POST_ADDRESS;
                DomainEntityDto.PaymentEssentialElements = PAYMENT;
                DomainEntityDto.Registered = REGISTERED;
                DomainEntityDto.Kpp = KPP;
                DomainEntityDto.SyncCode1C = SYNC_1C_CODE;
                DomainEntityDto.Timestamp = TIMESTAMP;

                Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOfficeOrganizationUnit>>())).Returns(Q(BranchOfficeOrganizationUnit));
            };

            It should_returns_expected_ShortLegalName = () => Result.ShortLegalName.Should().Be(SHORT_LEGAL_NAME);
            It should_returns_expected_BranchOfficeRefId = () => Result.BranchOfficeId.Should().Be(BRANCH_OFFICE_ID);
            It should_returns_expected_OrganizationUnitRefId = () => Result.OrganizationUnitId.Should().Be(ORG_UNIT_ID);
            It should_returns_expected_PhoneNumber = () => Result.PhoneNumber.Should().Be(PRONE_NUMBER);
            It should_returns_expected_ChiefNameInNominative = () => Result.ChiefNameInNominative.Should().Be(CHIEF_NAME_NOM);
            It should_returns_expected_ChiefNameInGenitive = () => Result.ChiefNameInGenitive.Should().Be(CHIEF_NAME_GEN);
            It should_returns_expected_PositionInNominative = () => Result.PositionInNominative.Should().Be(POSITION_NOM);
            It should_returns_expected_PositionInGenitive = () => Result.PositionInGenitive.Should().Be(POSITION_GEN);
            It should_returns_expected_OperatesOnTheBasisInGenitive = () => Result.OperatesOnTheBasisInGenitive.Should().Be(OPERTATES_GEN);
            It should_returns_expected_RegistrationCertificate = () => Result.RegistrationCertificate.Should().Be(REG_CERTIFICATE);
            It should_returns_expected_ActualAddress = () => Result.ActualAddress.Should().Be(ACTUAL_ADDRESS);
            It should_returns_expected_Email = () => Result.Email.Should().Be(EMAIL);
            It should_returns_expected_PostalAddress = () => Result.PostalAddress.Should().Be(POST_ADDRESS);
            It should_returns_expected_Registered = () => Result.Registered.Should().Be(REGISTERED);
            It should_returns_expected_Kpp = () => Result.Kpp.Should().Be(KPP);
            It should_returns_expected_Sync_1c_Code = () => Result.SyncCode1C.Should().Be(SYNC_1C_CODE);
            It should_returns_expected_PaymentEssentialElements = () => Result.PaymentEssentialElements.Should().Be(PAYMENT);
            It should_returns_expected_Timestamp = () => Result.Timestamp.Should().NotBeNull();
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeOrganizationUnitObtainer))]
        public class When_entity_exists_in_persistance : BranchOfficeOrganizationUnitObtainerResultContext
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
                Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOfficeOrganizationUnit>>())).Returns(Q(BranchOfficeOrganizationUnit));

                DomainEntityDto.CreatedByRef = TestCreatedBy;
                DomainEntityDto.ModifiedByRef = TestModifiedBy;
                DomainEntityDto.IsActive = TestIsActive;
                DomainEntityDto.IsDeleted = TestIsDeleted;
                DomainEntityDto.ModifiedOn = TestModifiedOn;
                DomainEntityDto.CreatedOn = TestCreatedOn;

                BranchOfficeOrganizationUnit.CreatedBy = CurrentCreatedBy;
                BranchOfficeOrganizationUnit.ModifiedBy = CurrentModifiedBy;
                BranchOfficeOrganizationUnit.IsActive = !TestIsActive;
                BranchOfficeOrganizationUnit.IsDeleted = !TestIsDeleted;
                BranchOfficeOrganizationUnit.CreatedOn = CurrentCreatedOn;
                BranchOfficeOrganizationUnit.ModifiedOn = CurrentModifiedOn;

                // Следующие строчки нужны, чтобы тест просто не упал
                DomainEntityDto.BranchOfficeRef = new EntityReference(1);
                DomainEntityDto.OrganizationUnitRef = new EntityReference(2);
            };

            private It should_not_change_entity_IsActive_Property = () => Result.IsActive.Should().Be(!TestIsActive);
            private It should_not_change_entity_IsDeleted_Property = () => Result.IsActive.Should().Be(!TestIsDeleted);
            private It should_not_change_entity_CreatedBy_Property = () => Result.CreatedBy.Should().Be(CurrentCreatedBy);
            private It should_not_change_entity_ModifiedBy_Property = () => Result.ModifiedBy.Should().Be(CurrentModifiedBy);
            private It should_not_change_entity_CreatedOn_Property = () => Result.CreatedOn.Should().Be(CurrentCreatedOn);
            private It should_not_change_entity_ModifiedOn_Property = () => Result.ModifiedOn.Should().Be(CurrentModifiedOn);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
