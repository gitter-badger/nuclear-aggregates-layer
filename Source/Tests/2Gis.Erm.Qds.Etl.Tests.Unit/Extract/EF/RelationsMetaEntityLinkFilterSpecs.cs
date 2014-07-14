using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

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
            Establish context = () => SetupTransformRelations(TestEntityName);

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
                    MetadataContainer = new Mock<IEntityToDocumentRelationMetadataContainer>();
                    Target = new RelationsMetaEntityLinkFilter(MetadataContainer.Object);
                };

            protected static void SetupTransformRelations(EntityName entityName, params Type[] relatedDocTypes)
            {
                var entityType = entityName.AsEntityType();

                var metadatas = relatedDocTypes.Select(relatedDocType =>
                {
                    var metadata = new Mock<IEntityToDocumentRelationMetadata>();
                    metadata.Setup(x => x.DocumentType).Returns(relatedDocType);
                    return metadata.Object;
                });

                MetadataContainer.Setup(t => t.GetMetadatasForEntityType(entityType)).Returns(metadatas);
            }

            protected static Mock<IEntityToDocumentRelationMetadataContainer> MetadataContainer { get; private set; }
            protected static RelationsMetaEntityLinkFilter Target { get; private set; }
            protected static EntityName TestEntityName { get; private set; }
        }
    }
}