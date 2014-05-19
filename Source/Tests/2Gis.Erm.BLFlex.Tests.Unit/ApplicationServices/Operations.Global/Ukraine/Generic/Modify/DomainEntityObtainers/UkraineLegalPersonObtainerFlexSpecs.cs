using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineLegalPersonObtainerFlexSpecs
    {
        public abstract class UkraineLegalPersonObtainerContext
        {
            protected static IFinder Finder;
            protected static IBusinessEntityPropertiesConverter<UkraineLegalPersonPart> LegalPersonPartConverter;
            protected static UkraineLegalPersonDomainEntityDto DomainEntityDto;

            protected static UkraineLegalPersonObtainerFlex UkraineLegalPersonObtainerFlex;

            protected static LegalPerson Target;

            Establish context = () =>
                {
                    Target = new LegalPerson() { Id = 1 };
                    DomainEntityDto = new UkraineLegalPersonDomainEntityDto();

                    Finder = Mock.Of<IFinder>();
                    LegalPersonPartConverter = Mock.Of<IBusinessEntityPropertiesConverter<UkraineLegalPersonPart>>();

                    UkraineLegalPersonObtainerFlex = new UkraineLegalPersonObtainerFlex(Finder, LegalPersonPartConverter);
                };
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineLegalPersonObtainerFlex))]
        public class When_getting_entity_parts_for_new_entity : UkraineLegalPersonObtainerContext
        {
            protected static IEntityPart[] Result;

            Because of = () => Result = UkraineLegalPersonObtainerFlex.GetEntityParts(Target);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<UkraineLegalPersonPart>().SingleOrDefault().Should().NotBeNull();

            It should_not_calls_property_converter = () => Mock.Get(LegalPersonPartConverter).Verify(x =>
                                                                                                     x.ConvertFromDynamicEntityInstance(
                                                                                                         Moq.It.IsAny<BusinessEntityInstance>(),
                                                                                                         Moq.It
                                                                                                            .IsAny<ICollection<BusinessEntityPropertyInstance>>()),
                                                                                                     Times.Never);
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineLegalPersonObtainerFlex))]
        public class When_getting_entity_parts_for_existing_entity : UkraineLegalPersonObtainerContext
        {
            protected static IEntityPart[] Result;
            protected static BusinessEntityInstanceDto TestBusinessEntityInstanceDto;
            protected static UkraineLegalPersonPart UkraineLegalPersonPart;

            Establish context = () =>
                {
                    Target.Timestamp = new byte[0]; // timestamp != null => сущность не новая
                    var businessEntityInstance = new BusinessEntityInstance { EntityId = Target.Id };
                    var propertyInstances = new BusinessEntityPropertyInstance[0];
                    TestBusinessEntityInstanceDto = new BusinessEntityInstanceDto
                        {
                            EntityInstance = businessEntityInstance,
                            PropertyInstances = propertyInstances
                        };

                    UkraineLegalPersonPart = new UkraineLegalPersonPart();

                    Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<LegalPerson>>())).Returns(Q(Target));

                    Mock.Get(Finder).Setup(x => x.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(
                        Moq.It.IsAny<ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto>>(),
                        Moq.It.IsAny<IFindSpecification<BusinessEntityInstance>>()))
                        .Returns(
                            (ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto> x, IFindSpecification<BusinessEntityInstance> y) =>
                            Q(TestBusinessEntityInstanceDto));

                    Mock.Get(LegalPersonPartConverter)
                        .Setup(x => x.ConvertFromDynamicEntityInstance(businessEntityInstance, propertyInstances))
                        .Returns(UkraineLegalPersonPart);
                };

            Because of = () => Result = UkraineLegalPersonObtainerFlex.GetEntityParts(Target);

            It should_returns_single_entity_part_instance = () => Result.Cast<UkraineLegalPersonPart>().SingleOrDefault().Should().Be(UkraineLegalPersonPart);

            It should_calls_property_converter = () => Mock.Get(LegalPersonPartConverter).Verify(x =>
                                                                                                 x.ConvertFromDynamicEntityInstance(
                                                                                                     Moq.It.IsAny<BusinessEntityInstance>(),
                                                                                                     Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()),
                                                                                                 Times.Once);
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineLegalPersonObtainerFlex))]
        public class When_calls_copy_fields : UkraineLegalPersonObtainerContext
        {
            const TaxationType TestTaxationType = TaxationType.WithVat;
            const string TestEgrpou = "TestEgrpou";

            static readonly byte[] TIMESTAMP = new byte[0];

            Establish context = () =>
                {
                    DomainEntityDto.TaxationType = TestTaxationType;
                    DomainEntityDto.Egrpou = TestEgrpou;

                    Target.Parts = new[] { new UkraineLegalPersonPart() };
                };

            Because of = () => UkraineLegalPersonObtainerFlex.CopyPartFields(Target, DomainEntityDto);

            It should_copy_TaxationType = () => Target.Parts.Cast<UkraineLegalPersonPart>().Single().TaxationType.Should().Be(TestTaxationType);
            It should_copy_Egrpou = () => Target.Parts.Cast<UkraineLegalPersonPart>().Single().Egrpou.Should().Be(TestEgrpou);
        }

        static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}