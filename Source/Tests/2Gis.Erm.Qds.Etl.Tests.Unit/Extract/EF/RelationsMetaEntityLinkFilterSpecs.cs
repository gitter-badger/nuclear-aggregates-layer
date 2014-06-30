using System;
using System.Collections.Generic;

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
            Establish context = () => SetupTransformRelations(TestEntityName, false);

            It should_not_be_supported = () => Target.IsSupported(new EntityLink(TestEntityName, 42)).Should().Be(false);
        }

        [Subject(typeof(RelationsMetaEntityLinkFilter))]
        class When_is_supported_for_supported_entity : RelationsMetaEntityLinkFilterContext
        {
            Establish context = () => SetupTransformRelations(TestEntityName, true, Mock.Of<Type>());

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

            protected static void SetupTransformRelations(EntityName entityName, bool success, params Type[] relatedDocTypes)
            {
                var relatedDocTypesEnumerable = (IEnumerable<Type>)relatedDocTypes;
                TransformRelations.Setup(t => t.TryGetRelatedDocTypes(entityName.AsEntityType(), out relatedDocTypesEnumerable)).Returns(success);
            }

            protected static Mock<ITransformRelations> TransformRelations { get; private set; }
            protected static RelationsMetaEntityLinkFilter Target { get; private set; }
            protected static EntityName TestEntityName { get; private set; }
        }
    }
}