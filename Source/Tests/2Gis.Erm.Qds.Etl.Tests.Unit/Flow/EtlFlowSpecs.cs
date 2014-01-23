using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Flow
{
    class EtlFlowSpecs
    {
        [Subject(typeof(EtlFlow))]
        class When_execute : EtlFlowContext
        {
            Because of = () => Target.Execute();

            It should_build_references = () => ReferencesBuilder.Verify(rb => rb.BuildReferences(Target));
        }

        [Subject(typeof(EtlFlow))]
        class When_references_built : EtlFlowContext
        {
            Establish context = () => _dataSource = Mock.Of<IDataSource>();

            Because of = () => Target.ReferencesBuilt(_dataSource);

            It should_extract_data = () => DataExtractor.Verify(e => e.Extract(_dataSource, Target));

            static IDataSource _dataSource;
        }

        [Subject(typeof(EtlFlow))]
        class When_data_extracted : EtlFlowContext
        {
            Establish context = () => _data = Mock.Of<IData>();

            Because of = () => Target.DataExtracted(_data);

            It should_transform = () => Transformation.Verify(t => t.Transform(_data, Target));

            static IData _data;
        }

        [Subject(typeof(EtlFlow))]
        class When_data_transformed : EtlFlowContext
        {
            Establish context = () => _transformedData = Mock.Of<ITransformedData>();

            Because of = () => Target.DataTransformed(_transformedData);

            It should_publish = () => Publisher.Verify(p => p.Publish(_transformedData));

            static ITransformedData _transformedData;
        }

        class EtlFlowContext
        {
            Establish context = () =>
                {
                    Publisher = new Mock<IPublisher>();
                    Transformation = new Mock<ITransformation>();
                    DataExtractor = new Mock<IExtractor>();
                    ReferencesBuilder = new Mock<IReferencesBuilder>();

                    Target = new EtlFlow(Publisher.Object, Transformation.Object, DataExtractor.Object, ReferencesBuilder.Object);
                };

            protected static Mock<IReferencesBuilder> ReferencesBuilder { get; private set; }
            protected static Mock<IExtractor> DataExtractor { get; private set; }
            protected static Mock<ITransformation> Transformation { get; private set; }
            protected static Mock<IPublisher> Publisher { get; private set; }

            protected static EtlFlow Target { get; private set; }
        }
    }
}
