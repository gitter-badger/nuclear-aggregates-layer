using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeOrganizationUnitDtoServiceSpecs
    {
        public abstract class UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            protected static UkraineGetBranchOfficeOrganizationUnitDtoService UkraineGetBranchOfficeOrganizationUnitDtoService;
            protected static IUserContext UserContext;
            protected static ISecureFinder SecureFinder;
            protected static IBranchOfficeReadModel ReadModel;
            protected static IAPIIdentityServiceSettings IdentityServiceSettings;
            protected static UkraineBranchOfficeOrganizationUnitDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    EntityId = 0;
                    ReadOnly = false;
                    ParentEntityId = null;
                    ParentEntityName = 0;
                    ExtendedInfo = null;

                    ReadModel = Mock.Of<IBranchOfficeReadModel>();
                    SecureFinder = Mock.Of<ISecureFinder>();
                    UserContext = Mock.Of<IUserContext>();
                    IdentityServiceSettings = Mock.Of<IAPIIdentityServiceSettings>();

                    UkraineGetBranchOfficeOrganizationUnitDtoService = new UkraineGetBranchOfficeOrganizationUnitDtoService(UserContext, SecureFinder, ReadModel, IdentityServiceSettings);
                };

            Because of = () =>
                {
                    Result = (UkraineBranchOfficeOrganizationUnitDomainEntityDto)UkraineGetBranchOfficeOrganizationUnitDtoService
                        .GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_existing_entity : UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            const long ENTITY_ID = 1;
            const long BRANCH_OFFICE_ADDL_ID = 2;
            const string IPN = "TestIPN";

            protected static UkraineBranchOfficeOrganizationUnitDomainEntityDto Dto;
            protected static BranchOffice BranchOffice;
            protected static UkraineBranchOfficePart UkraineBranchOfficePart;

            Establish context = () =>
                {
                    EntityId = ENTITY_ID;

                    Dto = new UkraineBranchOfficeOrganizationUnitDomainEntityDto { BranchOfficeAddlId = BRANCH_OFFICE_ADDL_ID };
                    UkraineBranchOfficePart = new UkraineBranchOfficePart { Ipn = IPN };
                    BranchOffice = new BranchOffice { Parts = new[] { UkraineBranchOfficePart } };

                    Mock.Get(ReadModel).Setup(x => x.GetBranchOfficeOrganizationUnitDto<UkraineBranchOfficeOrganizationUnitDomainEntityDto>(ENTITY_ID)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetBranchOffice(BRANCH_OFFICE_ADDL_ID)).Returns(BranchOffice);
                };

            It should_has_expected_EGRPOU = () => Result.BranchOfficeAddlIpn.Should().Be(IPN);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_new_entity : UkraineGetBranchOfficeOrganizationUnitDtoServiceContext
        {
            static readonly Uri RestUrl = new Uri("http://2gis.ru");
            static IUserIdentity _userIdentity;
            const long USER_CODE = 1;

            Establish context = () =>
            {
                EntityId = 0;

                Mock.Get(IdentityServiceSettings).Setup(x => x.RestUrl).Returns(RestUrl);

                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == USER_CODE);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            It should_be_UkraineBranchOfficeDomainEntityDto = () => Result.Should().BeOfType<UkraineBranchOfficeOrganizationUnitDomainEntityDto>();

            It should_has_expected_rest_url = () => Result.IdentityServiceUrl.Should().Be(RestUrl);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_new_entity_for_branch_office : When_requested_new_entity
        {
            protected static BranchOffice BranchOffice;

            const long PARENT_ENTITY_ID = 1;
            const string BRANCH_OFFICE_NAME = "TestName";

            Establish context = () =>
                {
                    ParentEntityId = PARENT_ENTITY_ID;
                    ParentEntityName = EntityName.BranchOffice;

                    BranchOffice = new BranchOffice { Name = BRANCH_OFFICE_NAME };

                    Mock.Get(ReadModel).Setup(x => x.GetBranchOffice(PARENT_ENTITY_ID)).Returns(BranchOffice);
                };

            It should_has_expected_branch_office_ref_name = () => Result.BranchOfficeRef.Name.Should().Be(BRANCH_OFFICE_NAME);

            It should_has_expected_branch_office_ref_Id = () => Result.BranchOfficeRef.Id.Should().Be(PARENT_ENTITY_ID);

            It should_has_expected_short_legal_name = () => Result.ShortLegalName.Should().Be(BRANCH_OFFICE_NAME);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_new_entity_for_organization_unit : When_requested_new_entity
        {
            protected static OrganizationUnit OrganizationUnit;

            const long PARENT_ENTITY_ID = 1;
            const string ORGANIZATION_UNIT_NAME = "TestName";

            Establish context = () =>
            {
                ParentEntityId = PARENT_ENTITY_ID;
                ParentEntityName = EntityName.OrganizationUnit;

                OrganizationUnit = new OrganizationUnit { Name = ORGANIZATION_UNIT_NAME };

                Mock.Get(SecureFinder).Setup(x => x.Find(Moq.It.IsAny<Expression<Func<OrganizationUnit, bool>>>())).Returns(Q(OrganizationUnit));
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
