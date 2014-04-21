using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
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
    public class ChileLegalPersonObtainerFlexSpecs
    {
        public abstract class ChileLegalPersonObtainerContext
        {
            protected static IFinder Finder;
            protected static IBusinessEntityPropertiesConverter<ChileLegalPersonPart> LegalPersonPartConverter;
            protected static ChileLegalPersonDomainEntityDto DomainEntityDto;

            protected static ChileLegalPersonObtainerFlex ChileLegalPersonObtainerFlex;

            protected static LegalPerson Target;

            Establish context = () =>
                {
                    Target = new LegalPerson() { Id = 1};
                    DomainEntityDto = new ChileLegalPersonDomainEntityDto();

                    Finder = Mock.Of<IFinder>();
                    LegalPersonPartConverter = Mock.Of<IBusinessEntityPropertiesConverter<ChileLegalPersonPart>>();

                    ChileLegalPersonObtainerFlex = new ChileLegalPersonObtainerFlex(Finder, LegalPersonPartConverter);
                };
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileLegalPersonObtainerFlex))]
        public class When_getting_entity_parts_for_new_entity : ChileLegalPersonObtainerContext
        {
            protected static IEntityPart[] Result;

            Because of = () => Result = ChileLegalPersonObtainerFlex.GetEntityParts(Target);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<ChileLegalPersonPart>().SingleOrDefault().Should().NotBeNull();

            It should_not_calls_property_converter = () => Mock.Get(LegalPersonPartConverter).Verify(x =>
                                                                                                     x.ConvertFromDynamicEntityInstance(
                                                                                                         Moq.It.IsAny<BusinessEntityInstance>(),
                                                                                                         Moq.It
                                                                                                            .IsAny<ICollection<BusinessEntityPropertyInstance>>()),
                                                                                                     Times.Never);
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileLegalPersonObtainerFlex))]
        public class When_getting_entity_parts_for_existing_entity : ChileLegalPersonObtainerContext
        {
            protected static IEntityPart[] Result;
            protected static BusinessEntityInstanceDto TestBusinessEntityInstanceDto;
            protected static ChileLegalPersonPart ChileLegalPersonPart;

            Establish context = () =>
                {
                    Target.Timestamp = new byte[0]; // timestamp != null => сущность не новая
                    var businessEntityInstance = new BusinessEntityInstance { EntityId = Target.Id };
                    var propertyInstances = new BusinessEntityPropertyInstance[0];
                    TestBusinessEntityInstanceDto = new BusinessEntityInstanceDto { EntityInstance = businessEntityInstance, PropertyInstances = propertyInstances };

                    ChileLegalPersonPart = new ChileLegalPersonPart();

                    Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<LegalPerson>>())).Returns(Q(Target));

                    Mock.Get(Finder).Setup(x => x.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(
                        Moq.It.IsAny<ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto>>(),
                        Moq.It.IsAny<IFindSpecification<BusinessEntityInstance>>()))
                        .Returns(
                            (ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto> x, IFindSpecification<BusinessEntityInstance> y) =>
                            Q(TestBusinessEntityInstanceDto));

                    Mock.Get(LegalPersonPartConverter).Setup(x => x.ConvertFromDynamicEntityInstance(businessEntityInstance, propertyInstances)).Returns(ChileLegalPersonPart);
                };

            Because of = () => Result = ChileLegalPersonObtainerFlex.GetEntityParts(Target);

            It should_returns_single_entity_part_instance = () => Result.Cast<ChileLegalPersonPart>().SingleOrDefault().Should().Be(ChileLegalPersonPart);

            It should_calls_property_converter = () => Mock.Get(LegalPersonPartConverter).Verify(x =>
                    x.ConvertFromDynamicEntityInstance(Moq.It.IsAny<BusinessEntityInstance>(), Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()), Times.Once);
        }

        [Tags("Obtainer")]
        [Subject(typeof(ChileLegalPersonObtainerFlex))]
        public class When_calls_copy_fields : ChileLegalPersonObtainerContext
        {
            const string TestOperationsKind = "TestOperationsKind";
            static readonly EntityReference TestCommuneRef = new EntityReference(100500, "TestCommune");

            static readonly byte[] TIMESTAMP = new byte[0];

            Establish context = () =>
                {
                    DomainEntityDto.OperationsKind = TestOperationsKind;
                    DomainEntityDto.CommuneRef = TestCommuneRef;

                    Target.Parts = new[] { new ChileLegalPersonPart() };
                };

            Because of = () => ChileLegalPersonObtainerFlex.CopyPartFields(Target, DomainEntityDto);

            It should_copy_OperationsKind = () => Target.Parts.Cast<ChileLegalPersonPart>().Single().OperationsKind.Should().Be(TestOperationsKind);
            It should_copy_Commune = () => Target.Parts.Cast<ChileLegalPersonPart>().Single().CommuneId.Should().Be(TestCommuneRef.Id);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}