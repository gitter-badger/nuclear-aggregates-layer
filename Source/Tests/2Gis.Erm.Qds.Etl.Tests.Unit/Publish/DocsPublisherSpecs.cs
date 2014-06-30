using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Publish
{
    public class DocsPublisherSpecs
    {
        // TODO {f.zaharov, 20.06.2014}: Тест на вызов Flush после

        [Subject(typeof(DocsPublisher))]
        public class When_publish_data : DocsPublisherContext
        {
            Establish context = () =>
            {
                _expectedDoc = Mock.Of<IDoc>();
                _expectedState = new RecordIdState("0", "0");
            };

            Because of = () => Target.Publish(new DenormalizedTransformedData(new[] { _expectedDoc }, _expectedState));

            It should_be_updated_to_storage = () =>
                DocsStorage.Verify(t => t.Update(Moq.It.Is<IEnumerable<IDoc>>(c => c.Contains(_expectedDoc))));

            It should_update_record_state_doc = () =>
                ChangesTrackerState.Verify(t => t.SetState(_expectedState));

            static IDoc _expectedDoc;
            static RecordIdState _expectedState;
        }

        [Subject(typeof(DocsPublisher))]
        public class When_publish_unsupported_type : DocsPublisherContext
        {
            Establish context = () => _unsupportedData = Mock.Of<ITransformedData>();

            Because of = () => _actualException = Catch.Exception(() => Target.Publish(_unsupportedData));

            It should_fail = () => _actualException.Should()
                 .NotBeNull("Ожидалось исключение.")
                 .And
                 .BeOfType<ArgumentException>("Не верный тип исключения.");

            private static Exception _actualException;
            private static ITransformedData _unsupportedData;
        }

        public class DocsPublisherContext
        {
            Establish context = () =>
                {
                    DocsStorage = new Mock<IDocsStorage>();
                    ChangesTrackerState = new Mock<IChangesTrackerState>();

                    Target = new DocsPublisher(DocsStorage.Object, ChangesTrackerState.Object);
                };

            protected static Mock<IChangesTrackerState> ChangesTrackerState { get; private set; }
            protected static DocsPublisher Target { get; private set; }
            protected static Mock<IDocsStorage> DocsStorage;
        }
    }
}