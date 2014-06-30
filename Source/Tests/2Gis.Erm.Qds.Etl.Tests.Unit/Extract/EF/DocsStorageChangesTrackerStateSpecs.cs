﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class DocsStorageChangesTrackerStateSpecs
    {
        [Subject(typeof(DocsStorageChangesTrackerState))]
        class When_get_by_id_throws_exception : DocsStorageChangesTrackerStateContext
        {
            Establish context = () =>
                {
                    ExpectedInner = new Exception();
                    DocsStorage.Setup(d => d.GetById<RecordIdState>(DocsStorageChangesTrackerState.StateRecordId))
                        .Throws(ExpectedInner);
                };

            Because of = () => Result = Catch.Exception(() => Target.GetState());

            It should_throw_specified_exception = () => Result.Should().BeOfType<TrackerStateException>();
            It should_contains_original_exception_as_inner = () => Result.InnerException.Should().Be(ExpectedInner);

            static Exception Result;
            static Exception ExpectedInner;
        }

        [Subject(typeof(DocsStorageChangesTrackerState))]
        class When_set_state_with_same_id : DocsStorageChangesTrackerStateContext
        {
            Establish context = () =>
                {
                    _oldRecordId = "42";
                    SetupDocStorageByState(new RecordIdState("0", _oldRecordId));
                };

            Because of = () => Target.SetState(new RecordIdState("0", _oldRecordId));

            It should_not_update_state = () => DocsStorage.Verify(s => s.Update(Moq.It.IsAny<IEnumerable<IDoc>>()), Times.Never);

            static string _oldRecordId;
        }

        [Subject(typeof(DocsStorageChangesTrackerState))]
        class When_set_state : DocsStorageChangesTrackerStateContext
        {
            Establish context = () =>
            {
                var oldState = new RecordIdState("0", "0");
                SetupDocStorageByState(oldState);

                _expectedState = new RecordIdState("0", "123");
            };

            Because of = () => Target.SetState(_expectedState);

            It should_update_record_id_doc = () => DocsStorage.Verify(s => s.Update(Moq.It.Is<IEnumerable<IDoc>>(d => d.Single() == _expectedState)));

            static RecordIdState _expectedState;
        }

        [Subject(typeof(DocsStorageChangesTrackerState))]
        class When_get_state : DocsStorageChangesTrackerStateContext
        {
            Establish context = () =>
                {
                    _expectedState = new RecordIdState("0", "123");
                    DocsStorage.Setup(d => d.GetById<RecordIdState>("0")).Returns(_expectedState);
                };

            Because of = () => _result = Target.GetState();

            It should_load_record_id_state_document = () => _result.Should().Be(_expectedState);

            static object _result;
            static RecordIdState _expectedState;
        }

        class DocsStorageChangesTrackerStateContext
        {
            Establish context = () =>
            {
                DocsStorage = new Mock<IDocsStorage>();

                Target = new DocsStorageChangesTrackerState(DocsStorage.Object);
            };

            protected static void SetupDocStorageByState(RecordIdState state)
            {
                if (state == null)
                {
                    throw new ArgumentNullException("state");
                }
                var query = Mock.Of<IDocsQuery>();

                DocsStorage.Setup(s => s.GetById<RecordIdState>(DocsStorageChangesTrackerState.StateRecordId)).Returns(state);
            }

            protected static Mock<IDocsStorage> DocsStorage { get; private set; }
            protected static DocsStorageChangesTrackerState Target { get; private set; }
        }
    }
}