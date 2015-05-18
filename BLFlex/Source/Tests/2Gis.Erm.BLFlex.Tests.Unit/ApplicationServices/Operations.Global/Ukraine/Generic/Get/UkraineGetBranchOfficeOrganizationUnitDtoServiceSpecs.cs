using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeOrganizationUnitDtoServiceSpecs
    {
        public abstract class UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            protected static UkraineGetBranchOfficeOrganizationUnitDtoService UkraineGetBranchOfficeOrganizationUnitDtoService;
            protected static IUserContext UserContext;
            protected static IOrganizationUnitReadModel OrganizationUnitReadModel;
            protected static IBranchOfficeReadModel BranchOfficeReadModel;
            protected static IIdentityServiceClientSettings IdentityServiceSettings;
            protected static UkraineBranchOfficeOrganizationUnitDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static IEntityType ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    EntityId = 0;
                    ReadOnly = false;
                    ParentEntityId = null;
                    ParentEntityName = EntityType.Instance.None();
                    ExtendedInfo = null;

                    BranchOfficeReadModel = Mock.Of<IBranchOfficeReadModel>();
                    OrganizationUnitReadModel = Mock.Of<IOrganizationUnitReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());
                    IdentityServiceSettings = Mock.Of<IIdentityServiceClientSettings>();

                    UkraineGetBranchOfficeOrganizationUnitDtoService = new UkraineGetBranchOfficeOrganizationUnitDtoService(UserContext, OrganizationUnitReadModel, BranchOfficeReadModel);
                };

            Because of = () =>
                {
                    Result = (UkraineBranchOfficeOrganizationUnitDomainEntityDto)UkraineGetBranchOfficeOrganizationUnitDtoService
                        .GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeOrganizationUnitDtoService))]
        class When_requested_existing_entity : UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            const long ENTITY_ID = 1;
            const long BRANCH_OFFICE_ADDL_ID = 2;
            const string IPN = "TestIPN";

            static BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit;
            static BranchOffice BranchOffice;

            Establish context = () =>
                {
                    EntityId = ENTITY_ID;

                    BranchOffice = new BranchOffice { Parts = new[] { new UkraineBranchOfficePart { Ipn = IPN } } };
                    BranchOfficeOrganizationUnit = new BranchOfficeOrganizationUnit { BranchOfficeId = BRANCH_OFFICE_ADDL_ID };

                    Mock.Get(BranchOfficeReadModel).Setup(x => x.GetBranchOfficeOrganizationUnit(ENTITY_ID)).Returns(BranchOfficeOrganizationUnit);
                    Mock.Get(BranchOfficeReadModel).Setup(x => x.GetBranchOffice(BRANCH_OFFICE_ADDL_ID)).Returns(BranchOffice);
                };

            It should_has_expected_IPN = () => Result.BranchOfficeAddlIpn.Should().Be(IPN);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeOrganizationUnitDtoService))]
        class When_requested_new_entity : UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            static readonly Uri RestUrl = new Uri("http://2gis.ru");

            Establish context = () =>
            {
                EntityId = 0;

                Mock.Get(IdentityServiceSettings).Setup(x => x.IdentityServiceUrl).Returns(RestUrl);
            };

            It should_be_UkraineBranchOfficeDomainEntityDto = () => Result.Should().BeOfType<UkraineBranchOfficeOrganizationUnitDomainEntityDto>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeOrganizationUnitDtoService))]
        class When_requested_new_entity_for_branch_office : When_requested_new_entity
        {
            protected static BranchOffice BranchOffice;

            const long PARENT_ENTITY_ID = 1;
            const string BRANCH_OFFICE_NAME = "TestName";

            Establish context = () =>
                {
                    ParentEntityId = PARENT_ENTITY_ID;
                    ParentEntityName = EntityType.Instance.BranchOffice();

                    BranchOffice = new BranchOffice { Name = BRANCH_OFFICE_NAME };

                    Mock.Get(BranchOfficeReadModel).Setup(x => x.GetBranchOffice(PARENT_ENTITY_ID)).Returns(BranchOffice);
                    Mock.Get(BranchOfficeReadModel).Setup(x => x.GetBranchOfficeName(PARENT_ENTITY_ID)).Returns(BRANCH_OFFICE_NAME);
                };

            It should_has_expected_branch_office_ref_name = () => Result.BranchOfficeRef.Name.Should().Be(BRANCH_OFFICE_NAME);

            It should_has_expected_branch_office_ref_Id = () => Result.BranchOfficeRef.Id.Should().Be(PARENT_ENTITY_ID);

            It should_has_expected_short_legal_name = () => Result.ShortLegalName.Should().Be(BRANCH_OFFICE_NAME);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeOrganizationUnitDtoService))]
        class When_requested_new_entity_for_organization_unit : When_requested_new_entity
        {
            const long PARENT_ENTITY_ID = 1;
            const string ORGANIZATION_UNIT_NAME = "TestName";

            Establish context = () =>
            {
                ParentEntityId = PARENT_ENTITY_ID;
                ParentEntityName = EntityType.Instance.OrganizationUnit();

                Mock.Get(OrganizationUnitReadModel).Setup(x => x.GetName(PARENT_ENTITY_ID)).Returns(ORGANIZATION_UNIT_NAME);
            };

            It should_has_expected_org_unit_ref_name = () => Result.OrganizationUnitRef.Name.Should().Be(ORGANIZATION_UNIT_NAME);

            It should_has_expected_org_unit_ref_Id = () => Result.OrganizationUnitRef.Id.Should().Be(PARENT_ENTITY_ID);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
