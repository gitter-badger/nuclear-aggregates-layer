using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileBranchOfficeOrganizationUnitObtainerFlexSpecs
    {
        public abstract class ChileBranchOfficeOrganizationUnitObtainerContext
        {
            protected static IFinder Finder;
            protected static IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> BranchOfficeOrganizationUnitPartConverter;
            protected static ChileBranchOfficeOrganizationUnitDomainEntityDto DomainEntityDto;

            protected static ChileBranchOfficeOrganizationUnitObtainerFlex ChileBranchOfficeOrganizationUnitObtainerFlex;

            protected static BranchOfficeOrganizationUnit Target;

            Establish context = () =>
                {
                    Target = new BranchOfficeOrganizationUnit();
                    DomainEntityDto = new ChileBranchOfficeOrganizationUnitDomainEntityDto();

                    Finder = Mock.Of<IFinder>();
                    BranchOfficeOrganizationUnitPartConverter = Mock.Of<IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart>>();

                    ChileBranchOfficeOrganizationUnitObtainerFlex = new ChileBranchOfficeOrganizationUnitObtainerFlex(Finder, BranchOfficeOrganizationUnitPartConverter);
                };
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileBranchOfficeOrganizationUnitObtainerFlex))]
        public class When_getting_entity_parts_for_new_entity : ChileBranchOfficeOrganizationUnitObtainerContext
        {
            protected static IEntityPart[] Result;

            Because of = () => Result = ChileBranchOfficeOrganizationUnitObtainerFlex.GetEntityParts(Target);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<ChileBranchOfficeOrganizationUnitPart>().SingleOrDefault().Should().NotBeNull();

            It should_not_calls_property_converter = () => Mock.Get(BranchOfficeOrganizationUnitPartConverter).Verify(x =>
                                                                                                      x.ConvertFromDynamicEntityInstance(Moq.It.IsAny<BusinessEntityInstance>(), Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()), Times.Never);
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileBranchOfficeOrganizationUnitObtainerFlex))]
        public class When_getting_entity_parts_for_existing_entity : ChileBranchOfficeOrganizationUnitObtainerContext
        {
            const long ENTITY_INSTANCE_ID = 1;

            protected static IEntityPart[] Result;
            protected static BusinessEntityInstanceDto BusinessEntityInstanceDto;
            protected static ChileBranchOfficeOrganizationUnitPart ChileBranchOfficeOrganizationUnitPart;

            Establish context = () =>
                {
                    Target.Timestamp = new byte[0]; // not null
                    var businessEntityInstance = new BusinessEntityInstance { EntityId = ENTITY_INSTANCE_ID };
                    var propertyInstances = new BusinessEntityPropertyInstance[0];
                    BusinessEntityInstanceDto = new BusinessEntityInstanceDto { EntityInstance = businessEntityInstance, PropertyInstances = propertyInstances };

                    ChileBranchOfficeOrganizationUnitPart = new ChileBranchOfficeOrganizationUnitPart();

                    Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOfficeOrganizationUnit>>())).Returns(Q(Target));

                    Mock.Get(Finder).Setup(x => x.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(
                        Moq.It.IsAny<ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto>>(),
                        Moq.It.IsAny<IFindSpecification<BusinessEntityInstance>>())).Returns(Q(BusinessEntityInstanceDto));

                    Mock.Get(BranchOfficeOrganizationUnitPartConverter).Setup(x => x.ConvertFromDynamicEntityInstance(businessEntityInstance, propertyInstances)).Returns(ChileBranchOfficeOrganizationUnitPart);
                };

            Because of = () => Result = ChileBranchOfficeOrganizationUnitObtainerFlex.GetEntityParts(Target);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<ChileBranchOfficeOrganizationUnitPart>().SingleOrDefault().Should().Be(ChileBranchOfficeOrganizationUnitPart);

            It should_returns_expected_instance_id = () => ChileBranchOfficeOrganizationUnitPart.EntityId.Should().Be(ENTITY_INSTANCE_ID);

            It should_calls_property_converter = () => Mock.Get(BranchOfficeOrganizationUnitPartConverter).Verify(x =>
                    x.ConvertFromDynamicEntityInstance(Moq.It.IsAny<BusinessEntityInstance>(), Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()), Times.Once);
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileBranchOfficeOrganizationUnitObtainerFlex))]
        public class When_calls_copy_fields : ChileBranchOfficeOrganizationUnitObtainerContext
        {
            const string RUT = "RUT";

            static readonly byte[] TIMESTAMP = new byte[0];
            
            protected static ChileBranchOfficeOrganizationUnitPart ChileBranchOfficeOrganizationUnitPart;
            protected static BusinessEntityInstanceDto BusinessEntityInstanceDto;

            Establish context = () =>
                {
                    DomainEntityDto.RepresentativeRut = RUT;

                    ChileBranchOfficeOrganizationUnitPart = new ChileBranchOfficeOrganizationUnitPart();
                    Target.Parts = new[] { ChileBranchOfficeOrganizationUnitPart };
                };

            Because of = () => ChileBranchOfficeOrganizationUnitObtainerFlex.CopyPartFields(Target, DomainEntityDto);

            It should_copy_RepresentativeRut = () => Target.Parts.Cast<ChileBranchOfficeOrganizationUnitPart>().Single().RepresentativeRut.Should().Be(RUT);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}