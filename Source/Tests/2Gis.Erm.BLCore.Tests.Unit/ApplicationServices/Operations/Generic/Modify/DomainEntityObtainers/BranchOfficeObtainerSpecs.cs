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
    public class BranchOfficeObtainerSpecs
    {
        public abstract class BranchOfficeObtainerContext
        {
            protected static IFinder Finder;
            protected static BranchOfficeDomainEntityDto DomainEntityDto;

            protected static BranchOfficeObtainer BranchOfficeObtainer;

            protected static BranchOffice BranchOffice;
            protected static EntityReference BargainTypeRef;
            protected static EntityReference ContributionTypeRef;

            Establish context = () =>
                {
                    BranchOffice = new BranchOffice();
                    BargainTypeRef = new EntityReference();
                    ContributionTypeRef = new EntityReference();
                    DomainEntityDto = new BranchOfficeDomainEntityDto
                        {
                            BargainTypeRef = BargainTypeRef,
                            ContributionTypeRef = ContributionTypeRef
                        };

                    Finder = Mock.Of<IFinder>();

                    BranchOfficeObtainer = new BranchOfficeObtainer(Finder);
                };
        }

        public class BranchOfficeObtainerResultContext : BranchOfficeObtainerContext
        {
            protected static BranchOffice Result;

            Because of = () => { Result = BranchOfficeObtainer.ObtainBusinessModelEntity(DomainEntityDto); };
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeObtainer))]
        public class When_create_existing_entity : BranchOfficeObtainerContext
        {
            protected static Exception catchedException;

            Establish context = () =>
                {
                    BranchOffice.Timestamp = new byte[0]; // not null

                    Mock.Get(Finder).Setup(x => x.FindOne(Moq.It.IsAny<IFindSpecification<BranchOffice>>())).Returns(BranchOffice);
                };

            Because of = () => catchedException = Catch.Exception(() => BranchOfficeObtainer.ObtainBusinessModelEntity(DomainEntityDto));

            It should_throw_business_logic_exception = () => catchedException.Should().BeOfType<BusinessLogicException>();
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeObtainer))]
        public class When_create_new_entity : BranchOfficeObtainerResultContext
        {
            const long DOMAIN_ENTITY_DTO_ID = 1;

            Establish context = () =>
                {
                    DomainEntityDto.Id = DOMAIN_ENTITY_DTO_ID;
                };

            It should_return_active_branch_office = () => Result.IsActive.Should().BeTrue();
            It should_return_not_deleted_branch_office = () => Result.IsDeleted.Should().BeFalse();

            It should_return_branch_office_with_expected_id = () => Result.Id.Should().Be(DOMAIN_ENTITY_DTO_ID);
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeObtainer))]
        public class When_obtain_branch_office : BranchOfficeObtainerResultContext
        {
            static readonly long? DGPP_ID = 1;
            const string NAME = "TestName";
            const string INN = "TestInn";
            const string ANNOTATION = "TestAnnotation";
            const string LEGAL_ADDRESS = "TestLegalAddress";
            const string USN_NOTIFICATION_TEXT = "TestNotificationText";
            static readonly byte[] TIMESTAMP = new byte[0];

            const long BARGAIN_TYPE_REF_ID = 2;
            const long CONTRIBUTION_TYPE_REF_ID = 3;

            Establish context = () =>
                {
                    DomainEntityDto.DgppId = DGPP_ID;
                    DomainEntityDto.Name = NAME;
                    DomainEntityDto.Inn = INN;
                    DomainEntityDto.Annotation = ANNOTATION;
                    DomainEntityDto.LegalAddress = LEGAL_ADDRESS;
                    DomainEntityDto.UsnNotificationText = USN_NOTIFICATION_TEXT;
                    DomainEntityDto.Timestamp = TIMESTAMP;

                    BargainTypeRef.Id = BARGAIN_TYPE_REF_ID;
                    ContributionTypeRef.Id = CONTRIBUTION_TYPE_REF_ID;

                    Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOffice>>())).Returns(Q(BranchOffice));
                };

            It should_return_expected_branch_office_DgppId = () => Result.DgppId.Should().Be(DGPP_ID);
            It should_return_expected_branch_office_Inn = () => Result.Inn.Should().Be(INN);
            It should_return_expected_branch_office_Annotation = () => Result.Annotation.Should().Be(ANNOTATION);
            It should_return_expected_branch_office_LegalAddress = () => Result.LegalAddress.Should().Be(LEGAL_ADDRESS);
            It should_return_expected_branch_office_UsnNotificationText = () => Result.UsnNotificationText.Should().Be(USN_NOTIFICATION_TEXT);
            It should_return_expected_branch_office_Timestamp = () => Result.Timestamp.Should().NotBeNull();
        }

        [Tags("Obtainer")]
        [Subject(typeof(BranchOfficeObtainer))]
        public class When_branch_office_exists_in_persistance : BranchOfficeObtainerResultContext
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
                Mock.Get(Finder).Setup(x => x.FindOne(Moq.It.IsAny<IFindSpecification<BranchOffice>>())).Returns(BranchOffice);

                DomainEntityDto.CreatedByRef = TestCreatedBy;
                DomainEntityDto.ModifiedByRef = TestModifiedBy;
                DomainEntityDto.IsActive = TestIsActive;
                DomainEntityDto.IsDeleted = TestIsDeleted;
                DomainEntityDto.ModifiedOn = TestModifiedOn;
                DomainEntityDto.CreatedOn = TestCreatedOn;

                BranchOffice.CreatedBy = CurrentCreatedBy;
                BranchOffice.ModifiedBy = CurrentModifiedBy;
                BranchOffice.IsActive = !TestIsActive;
                BranchOffice.IsDeleted = !TestIsDeleted;
                BranchOffice.CreatedOn = CurrentCreatedOn;
                BranchOffice.ModifiedOn = CurrentModifiedOn;
            };

            private It should_not_change_branch_office_IsActive_Property = () => Result.IsActive.Should().Be(!TestIsActive);
            private It should_not_change_branch_office_IsDeleted_Property = () => Result.IsActive.Should().Be(!TestIsDeleted);
            private It should_not_change_branch_office_CreatedBy_Property = () => Result.CreatedBy.Should().Be(CurrentCreatedBy);
            private It should_not_change_branch_office_ModifiedBy_Property = () => Result.ModifiedBy.Should().Be(CurrentModifiedBy);
            private It should_not_change_branch_office_CreatedOn_Property = () => Result.CreatedOn.Should().Be(CurrentCreatedOn);
            private It should_not_change_branch_office_ModifiedOn_Property = () => Result.ModifiedOn.Should().Be(CurrentModifiedOn);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}