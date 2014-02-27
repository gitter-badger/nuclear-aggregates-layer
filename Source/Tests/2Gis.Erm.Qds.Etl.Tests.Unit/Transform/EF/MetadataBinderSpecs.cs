using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    class MetadataBinderSpecs
    {
        [Subject(typeof(MetadataBinder))]
        class When_bind_metadata : MetadataBinderContext
        {
            Establish context = () =>
                {
                    ExpectedUpdater = Mock.Of<IDocsUpdater>();
                    ExpectedDocRelation = Mock.Of<IDocRelation>();

                    var qds = new Mock<IQdsComponent>();
                    qds.Setup(q => q.CreateDocUpdater()).Returns(ExpectedUpdater);
                    qds.SetupGet(q => q.PartsDocRelation).Returns(ExpectedDocRelation);

                    QdsComponents = new [] { qds.Object };
                };

            Because of = () => Target.BindMetadata(QdsComponents);

            It should_add_modifier = () => Updaters.Verify(m => m.AddUpdater(ExpectedUpdater));
            It should_register_relations = () => Relations.Verify(r => r.RegisterRelation(ExpectedDocRelation));

            private static IDocsUpdater ExpectedUpdater;
            private static IDocRelation ExpectedDocRelation;
        }

        class MetadataBinderContext
        {
            Establish context = () =>
                {
                    Relations = new Mock<ITransformRelations>();
                    Updaters = new Mock<IDocUpdatersRegistry>();

                    Target = new MetadataBinder(Updaters.Object, Relations.Object);
                };

            protected static Mock<ITransformRelations> Relations { get; private set; }
            protected static Mock<IDocUpdatersRegistry> Updaters { get; private set; }
            protected static IEnumerable<IQdsComponent> QdsComponents { get; set; }

            protected static MetadataBinder Target { get; private set; }
        }
    }
}