using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class OperationsLogChangesCollectorSpecs
    {
        [Subject(typeof(OperationsLogChangesCollector))]
        class When_load_changes_with_record_id_state : OperationsLogChangesCollectorContext
        {
            Establish context = () =>
                {
                    _lastId = 42;
                    _expectedOperation = new PerformedBusinessOperation { Id = 43 };

                    Finder.Setup(f => f.Find(Moq.It.IsAny<Expression<Func<PerformedBusinessOperation, bool>>>()))
                          .Returns(new[] { _expectedOperation }.AsQueryable())
                          .Callback<Expression<Func<PerformedBusinessOperation, bool>>>(exp => _expression = exp);

                };

            Because of = () => Result = Target.LoadChanges(new RecordIdState("0", _lastId.ToString()));

            It should_return_new_records = () => Result.Should().OnlyContain(cr => ((PboChangeDescriptor)cr).Operation == _expectedOperation);
            It should_pass_expected_pbo_for_query_expression = () => _expression.Compile()(_expectedOperation).Should().Be(true);
            It should_return_new_state = () => Target.CurrentState.As<RecordIdState>().RecordId.Should().Be(_expectedOperation.Id.ToString());

            static PerformedBusinessOperation _expectedOperation;
            static long _lastId;
            static Expression<Func<PerformedBusinessOperation, bool>> _expression;
            static IEnumerable<IChangeDescriptor> Result { get; set; }
        }

        [Subject(typeof(OperationsLogChangesCollector))]
        class When_load_changes_with_unsupported_state : OperationsLogChangesCollectorContext
        {
            Because of = () => _catchedEx = Catch.Exception(() => Target.LoadChanges(Mock.Of<ITrackState>()).ToArray());

            It should_throw_not_supported_exception = () => _catchedEx.Should().NotBeNull().And.BeOfType<NotSupportedException>();

            static Exception _catchedEx;
        }

        class OperationsLogChangesCollectorContext
        {
            Establish context = () =>
                {
                    Finder = new Mock<IFinder>();
                    Target = new OperationsLogChangesCollector(Finder.Object);
                };

            protected static Mock<IFinder> Finder { get; private set; }
            protected static OperationsLogChangesCollector Target { get; private set; }
        }
    }
}