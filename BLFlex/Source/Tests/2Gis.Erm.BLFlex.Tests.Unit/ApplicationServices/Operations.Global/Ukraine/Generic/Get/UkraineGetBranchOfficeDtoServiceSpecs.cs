using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.NuClear.IdentityService.Client.Settings;
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
            protected static IBargainTypeReadModel BargainTypeReadModel;
            protected static IContributionTypeReadModel ContributionTypeReadModel;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ReadModel = Mock.Of<IBranchOfficeReadModel>();
                    BargainTypeReadModel = Mock.Of<IBargainTypeReadModel>();
                    ContributionTypeReadModel = Mock.Of<IContributionTypeReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());

                    UkraineGetBranchOfficeDtoService = new UkraineGetBranchOfficeDtoService(UserContext, ReadModel, BargainTypeReadModel, ContributionTypeReadModel);
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
            protected static BranchOffice BranchOffice;
            protected static UkraineBranchOfficePart UkraineBranchOfficePart;

            const string IPN = "TestIPN";
            const long ENTITY_ID = 1;

            Establish context = () =>
                {
                    EntityId = ENTITY_ID;

                    UkraineBranchOfficePart = new UkraineBranchOfficePart { Ipn = IPN };
                    BranchOffice = new BranchOffice { Parts = new[] { UkraineBranchOfficePart } };

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

            Establish context = () =>
            {
                EntityId = 0;
            };

            It should_be_UkraineBranchOfficeDomainEntityDto = () => Result.Should().BeOfType<UkraineBranchOfficeDomainEntityDto>();
        }
    }
}
