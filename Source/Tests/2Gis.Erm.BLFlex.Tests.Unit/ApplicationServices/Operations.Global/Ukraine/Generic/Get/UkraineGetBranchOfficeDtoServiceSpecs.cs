using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeDtoServiceSpecs
    {
        public abstract class UkraineGetBranchOfficeDtoServiceContext
        {
            protected static UkraineGetBranchOfficeDtoService UkraineGetBranchOfficeDtoService;
            protected static IUserContext UserContext;
            protected static IBranchOfficeReadModel ReadModel;
            protected static IAPIIdentityServiceSettings IdentityServiceSettings;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ReadModel = Mock.Of<IBranchOfficeReadModel>();
                    UserContext = Mock.Of<IUserContext>();
                    IdentityServiceSettings = Mock.Of<IAPIIdentityServiceSettings>();

                    UkraineGetBranchOfficeDtoService = new UkraineGetBranchOfficeDtoService(UserContext, ReadModel, IdentityServiceSettings);
                };

            Because of = () =>
                {
                    Result = UkraineGetBranchOfficeDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_existing_entity : UkraineGetBranchOfficeDtoServiceContext
        {
            protected static UkraineBranchOfficeDomainEntityDto Dto;
            protected static BranchOffice BranchOffice;
            protected static UkraineBranchOfficePart UkraineBranchOfficePart;

            const string IPN = "TestIPN";
            const long ENTITY_ID = 1;

            Establish context = () =>
                {
                    EntityId = ENTITY_ID;
                    Dto = new UkraineBranchOfficeDomainEntityDto();

                    UkraineBranchOfficePart = new UkraineBranchOfficePart { Ipn = IPN };
                    BranchOffice = new BranchOffice { Parts = new[] { UkraineBranchOfficePart } };

                    Mock.Get(ReadModel).Setup(x => x.GetBranchOfficeDto<UkraineBranchOfficeDomainEntityDto>(ENTITY_ID)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetBranchOffice(EntityId)).Returns(BranchOffice);
                };

            It should_be_UkraineBranchOfficeDomainEntityDto = () => Result.Should().BeOfType<UkraineBranchOfficeDomainEntityDto>();

            It should_has_expected_EGRPOU = () => ((UkraineBranchOfficeDomainEntityDto)Result).Ipn.Should().Be(IPN);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetBranchOfficeDtoService))]
        class When_requested_new_entity : UkraineGetBranchOfficeDtoServiceContext
        {
            static readonly Uri RestUrl = new Uri("http://2gis.ru");
            static IUserIdentity _userIdentity;
            const long USER_CODE = 2;

            Establish context = () =>
            {
                EntityId = 0;

                Mock.Get(IdentityServiceSettings).Setup(x => x.RestUrl).Returns(RestUrl);

                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == USER_CODE);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            It should_be_UkraineBranchOfficeDomainEntityDto = () => Result.Should().BeOfType<UkraineBranchOfficeDomainEntityDto>();

            It should_has_expected_rest_url = () => ((UkraineBranchOfficeDomainEntityDto)Result).IdentityServiceUrl.Should().Be(RestUrl);
        }
    }
}
