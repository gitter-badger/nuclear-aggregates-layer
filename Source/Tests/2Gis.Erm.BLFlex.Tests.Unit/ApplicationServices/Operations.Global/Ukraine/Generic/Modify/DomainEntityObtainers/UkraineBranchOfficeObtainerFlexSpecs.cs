using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineBranchOfficeObtainerFlexSpecs
    {
        public abstract class UkraineBranchOfficeObtainerContext
        {
            protected static IFinder Finder;
            protected static IBusinessEntityPropertiesConverter<UkraineBranchOfficePart> BranchOfficePartConverter;
            protected static UkraineBranchOfficeDomainEntityDto DomainEntityDto;

            protected static UkraineBranchOfficeObtainerFlex UkraineBranchOfficeObtainerFlex;

            protected static BranchOffice BranchOffice;
            protected static EntityReference BargainTypeRef;
            protected static EntityReference ContributionTypeRef;

            Establish context = () =>
                {
                    BranchOffice = new BranchOffice();
                    BargainTypeRef = new EntityReference();
                    ContributionTypeRef = new EntityReference();
                    DomainEntityDto = new UkraineBranchOfficeDomainEntityDto
                        {
                            BargainTypeRef = BargainTypeRef,
                            ContributionTypeRef = ContributionTypeRef
                        };

                    Finder = Mock.Of<IFinder>();
                    BranchOfficePartConverter = Mock.Of<IBusinessEntityPropertiesConverter<UkraineBranchOfficePart>>();

                    UkraineBranchOfficeObtainerFlex = new UkraineBranchOfficeObtainerFlex(Finder, BranchOfficePartConverter);
                };
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineBranchOfficeObtainerFlex))]
        public class When_getting_entity_parts_for_new_entity : UkraineBranchOfficeObtainerContext
        {
            protected static IEntityPart[] Result;

            Because of = () => Result = UkraineBranchOfficeObtainerFlex.GetEntityParts(BranchOffice);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<UkraineBranchOfficePart>().SingleOrDefault().Should().NotBeNull();

            It should_not_calls_property_converter = () => Mock.Get(BranchOfficePartConverter).Verify(x => 
                x.ConvertFromDynamicEntityInstance(Moq.It.IsAny<BusinessEntityInstance>(), Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()), Times.Never);
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineBranchOfficeObtainerFlex))]
        public class When_getting_entity_parts_for_existing_entity : UkraineBranchOfficeObtainerContext
        {
            const long ENTITY_INSTANCE_ID = 1;

            protected static IEntityPart[] Result;
            protected static BusinessEntityInstanceDto BusinessEntityInstanceDto;
            protected static UkraineBranchOfficePart UkraineBranchOfficePart;

            Establish context = () =>
            {
                BranchOffice.Timestamp = new byte[0]; // not null
                var businessEntityInstance = new BusinessEntityInstance { EntityId = ENTITY_INSTANCE_ID };
                var propertyInstances = new BusinessEntityPropertyInstance[0];
                BusinessEntityInstanceDto = new BusinessEntityInstanceDto { EntityInstance = businessEntityInstance, PropertyInstances = propertyInstances};

                UkraineBranchOfficePart = new UkraineBranchOfficePart();

                Mock.Get(Finder).Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<BranchOffice>>())).Returns(Q(BranchOffice));

                Mock.Get(Finder).Setup(x => x.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(
                    Moq.It.IsAny<ISelectSpecification<BusinessEntityInstance, BusinessEntityInstanceDto>>(),
                    Moq.It.IsAny<IFindSpecification<BusinessEntityInstance>>())).Returns(Q(BusinessEntityInstanceDto));

                Mock.Get(BranchOfficePartConverter).Setup(x => x.ConvertFromDynamicEntityInstance(businessEntityInstance, propertyInstances)).Returns(UkraineBranchOfficePart);
            };

            Because of = () => Result = UkraineBranchOfficeObtainerFlex.GetEntityParts(BranchOffice);

            It should_returns_new_single_entity_part_instance = () => Result.Cast<UkraineBranchOfficePart>().SingleOrDefault().Should().Be(UkraineBranchOfficePart);

            It should_returns_expected_instance_id = () => UkraineBranchOfficePart.EntityId.Should().Be(ENTITY_INSTANCE_ID);

            It should_calls_property_converter = () => Mock.Get(BranchOfficePartConverter).Verify(x => 
                x.ConvertFromDynamicEntityInstance(Moq.It.IsAny<BusinessEntityInstance>(), Moq.It.IsAny<ICollection<BusinessEntityPropertyInstance>>()), Times.Once);
        }

        [Tags("Obtainer")]
        [Subject(typeof(UkraineBranchOfficeObtainerFlex))]
        public class When_calls_copy_fields : UkraineBranchOfficeObtainerContext
        {
            protected const string IPN = "TestIPN";

            protected static UkraineBranchOfficePart UkraineBranchOfficePart;

            Establish context = () =>
                {
                    DomainEntityDto.Ipn = IPN;

                    UkraineBranchOfficePart = new UkraineBranchOfficePart();
                    BranchOffice.Parts = new[] { UkraineBranchOfficePart };
                };

            Because of = () => UkraineBranchOfficeObtainerFlex.CopyPartFields(BranchOffice, DomainEntityDto);

            It should_copy_Egrpou = () => BranchOffice.Parts.Cast<UkraineBranchOfficePart>().Single().Ipn.Should().Be(IPN);
        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}
