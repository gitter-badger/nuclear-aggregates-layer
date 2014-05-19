using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

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
            protected static ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> CreateService;
            protected static IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit> UpdateService;
            protected static IPartableEntityValidator<BranchOfficeOrganizationUnit> Validator;

            protected static IDomainEntityDto DomainEntityDto;

            protected static ModifyBranchOfficeOrganizationUnitService ModifyBranchOfficeOrganizationUnitService;

            protected static BranchOfficeOrganizationUnit Entity;
            protected static IEnumerable<BusinessEntityInstanceDto> PartDtos;

            Establish context = () =>
                {
                    Entity = new BranchOfficeOrganizationUnit { Id = ENTITY_ID };
                    PartDtos = new BusinessEntityInstanceDto[0];

                    DomainEntityDto = Mock.Of<IDomainEntityDto>();

                    ReadModel = Mock.Of<IBranchOfficeReadModel>(x => x.GetBusinessEntityInstanceDto(Entity) == PartDtos);
                    Obtainer = Mock.Of<IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>>(x => x.ObtainBusinessModelEntity(DomainEntityDto) == Entity);
                    CreateService = Mock.Of<ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit>>();
                    UpdateService = Mock.Of<IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit>>();
                    Validator = Mock.Of<IPartableEntityValidator<BranchOfficeOrganizationUnit>>();

                    ModifyBranchOfficeOrganizationUnitService = new ModifyBranchOfficeOrganizationUnitService(ReadModel, Obtainer, CreateService, UpdateService, Validator);
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
                x.Create(Moq.It.IsAny<BranchOfficeOrganizationUnit>(), Moq.It.IsAny<IEnumerable<BusinessEntityInstanceDto>>()), Times.Never);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x => 
                x.Update(Moq.It.IsAny<BranchOfficeOrganizationUnit>(), Moq.It.IsAny<IEnumerable<BusinessEntityInstanceDto>>()), Times.Never);
        }

        [Tags("ModifyService")]
        [Subject(typeof(ModifyBranchOfficeOrganizationUnitService))]
        class When_create_new_entity : ModifyBranchOfficeOrganizationUnitServiceResultContext
        {
            It should_calls_validator = () => Mock.Get(Validator).Verify(x => x.Check(Entity), Times.Once);

            It should_calls_create_service = () => Mock.Get(CreateService).Verify(x => x.Create(Entity, PartDtos), Times.Once);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x =>
                x.Update(Moq.It.IsAny<BranchOfficeOrganizationUnit>(), Moq.It.IsAny<IEnumerable<BusinessEntityInstanceDto>>()), Times.Never);

            It should_returns_entity_id = () => Result.Should().Be(ENTITY_ID);
        }

        [Tags("ModifyService")]
        [Subject(typeof(ModifyBranchOfficeOrganizationUnitService))]
        class When_modify_entity : ModifyBranchOfficeOrganizationUnitServiceResultContext
        {
            Establish context = () => Entity.Timestamp = new byte[0]; // not null

            It should_calls_validator = () => Mock.Get(Validator).Verify(x => x.Check(Entity), Times.Once);

            It should_calls_create_service = () => Mock.Get(CreateService).Verify(x =>
                x.Create(Moq.It.IsAny<BranchOfficeOrganizationUnit>(), Moq.It.IsAny<IEnumerable<BusinessEntityInstanceDto>>()), Times.Never);

            It should_not_calls_update_service = () => Mock.Get(UpdateService).Verify(x =>
                x.Update(Entity, PartDtos), Times.Once);

            It should_returns_entity_id = () => Result.Should().Be(ENTITY_ID);
        }
    }
}
