using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class RelationsMetaEntityLinkFilterSpecs
    {
        [Subject(typeof(RelationsMetaEntityLinkFilter))]
        class When_is_supported_for_not_supported_entity : RelationsMetaEntityLinkFilterContext
        {
            Establish context = () => SetupTransformRelations(TestEntityName, new Type[0]);

            It should_not_be_supported = () => Target.IsSupported(new EntityLink(TestEntityName, 42)).Should().Be(false);
        }

        [Subject(typeof(RelationsMetaEntityLinkFilter))]
        class When_is_supported_for_supported_entity : RelationsMetaEntityLinkFilterContext
        {
            Establish context = () => SetupTransformRelations(TestEntityName, Mock.Of<Type>());

            It should_be_supported = () => Target.IsSupported(new EntityLink(TestEntityName, 42)).Should().Be(true);
        }

        class RelationsMetaEntityLinkFilterContext
        {
            Establish context = () =>
                {
                    TestEntityName = EntityName.Client;
                    TransformRelations = new Mock<ITransformRelations>();
                    Target = new RelationsMetaEntityLinkFilter(TransformRelations.Object);
                };

            protected static void SetupTransformRelations(EntityName entityName, params Type[] relatedDocTypes)
            {
                TransformRelations.Setup(t => t.GetRelatedDocTypes(entityName.AsEntityType())).Returns(relatedDocTypes);
            }

            protected static Mock<ITransformRelations> TransformRelations { get; private set; }
            protected static RelationsMetaEntityLinkFilter Target { get; private set; }
            protected static EntityName TestEntityName { get; private set; }
        }
    }
}