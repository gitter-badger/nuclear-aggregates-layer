using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities.Aspects;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.MultiCulture.Generic.Modify
{
    class ModifyBranchOfficeOrganizationUnitServiceSpecs
    {
        public abstract class ModifyBranchOfficeOrganizationUnitServiceContext
        {
            protected const long ENTITY_ID = 1;

            protected static IBranchOfficeReadModel ReadModel;
            protected static IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit> Obtainer;
            protected static ICreateAggregateRepository<BranchOfficeOrganizationUnit> CreateService;
            protected static IUpdateAggregateRepository<BranchOfficeOrganizationUnit> UpdateService;
            protected static IPartableEntityValidator<BranchOfficeOrganizationUnit> Validator;
            protected static Mock<IBranchOfficeReadModel> BranchOfficeReadModelMock;
            protected static Mock<IOrganizationUnitReadModel> OrganizationUnitReadModelMock;

            protected static IDomainEntityDto DomainEntityDto;

            protected static ModifyBranchOfficeOrganizationUnitService ModifyBranchOfficeOrganizationUnitService;

            protected static BranchOfficeOrganizationUnit Entity;

            Establish context = () =>
                {
                    Entity = new BranchOfficeOrganizationUnit { Id = ENTITY_ID };

                    DomainEntityDto = Mock.Of<IDomainEntityDto>();

                    BranchOfficeReadModelMock = new Mock<IBranchOfficeReadModel>();
                    BranchOfficeReadModelMock.Setup(x => x.GetBranchOffice(Moq.It.IsAny<long>()))
                                             .Returns(new BranchOffice() { IsActive = true, IsDeleted = false });

                    BranchOfficeReadModelMock.Setup(x => x.GetPrimaryBranchOfficeOrganizationUnits(Moq.It.IsAny<long>()))
                                             .Returns(new PrimaryBranchOfficeOrganizationUnits() { });

                    OrganizationUnitReadModelMock = new Mock<IOrganizationUnitReadModel>();
                    OrganizationUnitReadModelMock.Setup(x => x.GetOrganizationUnit(Moq.It.IsAny<long>()))
                                                 .Returns(new OrganizationUnit() { IsActive = true, IsDeleted = false });

                    Obtainer = Mock.Of<IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>>(x => x.ObtainBusinessModelEntity(DomainEntityDto) == Entity);
                    CreateService = Mock.Of<ICreateAggregateRepository<BranchOfficeOrganizationUnit>>();
                    UpdateService = Mock.Of<IUpdateAggregateRepository<BranchOfficeOrganizationUnit>>();
                    Validator = Mock.Of<IPartableEntityValidator<BranchOfficeOrganizationUnit>>();

                    ModifyBranchOfficeOrganizationUnitService = new ModifyBranchOfficeOrganizationUnitService(Obtainer,
                                                                                                              CreateService,
                                                                                                              UpdateService,
                                                                                                              Validator,
                                                                                                              BranchOfficeReadModelMock.Object,
                                                                                                              OrganizationUnitReadModelMock.Object);
                };
        }

        public abstract class ModifyBranchOfficeOrganizationUnitServiceResultContext : ModifyBranchOfficeOrganizationUnitServiceContext
        {
            protected static long Result;

            Because of = () => Result = ModifyBranchOfficeOrganizationUnitService.Modify(DomainEntityDto);
        }

        [Tags("ModifyService")]
        [Subject(typeof(ModifyBranchOfficeOrganizationUnitService))]
        class When_validator_throws_exception : ModifyBranchOfficeOrganizationUnitServiceContext
        {
            Establish context = () => Mock.Get(Validator).Setup(x => x.Check(Entity)).Throws<Exception>();

            Because of = () => Catch.Exception(() => ModifyBranchOfficeOrganizationUnitService.Modify(DomainEntityDto));

            It should_not_calls_create_service = () => Mock.Get(CreateService).Verify(x => 
                x.Create(Moq.It.IsAny<BranchOfficeOrganizationUnit>()), Times.Never);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x => 
                x.Update(Moq.It.IsAny<BranchOfficeOrganizationUnit>()), Times.Never);
        }

        [Tags("ModifyService")]
        [Subject(typeof(ModifyBranchOfficeOrganizationUnitService))]
        class When_create_new_entity : ModifyBranchOfficeOrganizationUnitServiceResultContext
        {
            It should_calls_validator = () => Mock.Get(Validator).Verify(x => x.Check(Entity), Times.Once);

            It should_calls_create_service = () => Mock.Get(CreateService).Verify(x => x.Create(Entity), Times.Once);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x =>
                x.Update(Moq.It.IsAny<BranchOfficeOrganizationUnit>()), Times.Never);

            It should_returns_entity_id = () => Result.Should().Be(ENTITY_ID);
        }

        [Tags("ModifyService")]
        [Subject(typeof(ModifyBranchOfficeOrganizationUnitService))]
        class When_modify_entity : ModifyBranchOfficeOrganizationUnitServiceResultContext
        {
            Establish context = () => Entity.Timestamp = new byte[0]; // not null

            It should_calls_validator = () => Mock.Get(Validator).Verify(x => x.Check(Entity), Times.Once);

            It should_calls_create_service = () => Mock.Get(CreateService).Verify(x =>
                x.Create(Moq.It.IsAny<BranchOfficeOrganizationUnit>()), Times.Never);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x =>
                x.Update(Entity), Times.Once);

            It should_returns_entity_id = () => Result.Should().Be(ENTITY_ID);
        }
    }
}
