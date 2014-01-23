using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class ChangesReferencesBuilderSpecs
    {
        [Subject(typeof(ChangesReferencesBuilder))]
        class When_track_changes_with_unsupported_entity_link : ChangesReferencesBuilderContext
        {
            Establish context = () =>
            {
                var changeDescriptor = Mock.Of<IChangeDescriptor>();
                SetupChanges(changeDescriptor);

                _expectedEntityLink = new EntityLink(EntityName.Client, 42);
                var unexpectedEntityLink = new EntityLink(EntityName.Account, 42);
                SetupEntityLinkBuilder(changeDescriptor, _expectedEntityLink, unexpectedEntityLink);

                EntityLinkFilter.Setup(f => f.IsSupported(_expectedEntityLink)).Returns(true);
            };

            Because of = () => TargetBuildReferences();

            It should_be_filtered = () => Result.Links.Should().OnlyContain(el => el == _expectedEntityLink);

            static EntityLink _expectedEntityLink;
        }

        [Subject(typeof(ChangesReferencesBuilder))]
        class When_track_changes_for_no_changes : ChangesReferencesBuilderContext
        {
            Establish context = () =>
            {
                SetupChanges(new IChangeDescriptor[0]);
                SetupEntityLinkBuilder(Moq.It.IsAny<IChangeDescriptor>(), new EntityLink(EntityName.Client, 42));
            };

            Because of = () => TargetBuildReferences();

            It should_return_empty_list = () => Result.Links.Should().BeEmpty();
        }

        [Subject(typeof(ChangesReferencesBuilder))]
        class When_track_changes : ChangesReferencesBuilderContext
        {
            Establish context = () =>
                {
                    _expectedNewState = Mock.Of<ITrackState>();
                    ChangesCollector.Setup(cc => cc.CurrentState).Returns(_expectedNewState);

                    var changeDescriptor = Mock.Of<IChangeDescriptor>();
                    SetupChanges(changeDescriptor);

                    _expectedEntityLink = new EntityLink(EntityName.Client, 42);
                    SetupEntityLinkBuilder(changeDescriptor, _expectedEntityLink);

                    EntityLinkFilter.Setup(f => f.IsSupported(_expectedEntityLink)).Returns(true);
                };

            Because of = () => TargetBuildReferences();

            It should_return_entity_links_for_changes = () => Result.Links.Should().OnlyContain(el => el == _expectedEntityLink);
            It should_return_new_state = () => Result.State.Should().Be(_expectedNewState);

            static EntityLink _expectedEntityLink;
            static ITrackState _expectedNewState;
        }

        class ChangesReferencesBuilderContext
        {
            Establish context = () =>
                {
                    _refConsumer = new Mock<IReferencesConsumer>();

                    _refConsumer.Setup(r => r.ReferencesBuilt(Moq.It.IsAny<IDataSource>()))
                        .Callback((IDataSource ds) => Result = (EntityLinksDataSource)ds);

                    ChangesCollector = new Mock<IChangesCollector>();
                    EntityLinkBuilder = new Mock<IEntityLinkBuilder>();
                    TrackerState = new Mock<IChangesTrackerState>();

                    ChangesCollector.Setup(cc => cc.CurrentState).Returns(Mock.Of<ITrackState>);

                    EntityLinkFilter = new Mock<IEntityLinkFilter>();

                    Target = new ChangesReferencesBuilder(EntityLinkBuilder.Object, ChangesCollector.Object, TrackerState.Object, EntityLinkFilter.Object);
                };

            static Mock<IReferencesConsumer> _refConsumer;

            protected static void TargetBuildReferences()
            {
                Target.BuildReferences(_refConsumer.Object);
            }

            protected static void SetupEntityLinkBuilder(IChangeDescriptor change, params EntityLink[] links)
            {
                EntityLinkBuilder.Setup(b => b.CreateEntityLinks(change)).Returns(links);
            }

            protected static void SetupChanges(params IChangeDescriptor[] changes)
            {
                var state = Mock.Of<ITrackState>();
                TrackerState.Setup(s => s.GetState()).Returns(state);

                ChangesCollector.Setup(cc => cc.LoadChanges(state)).Returns(changes);
            }

            protected static Mock<IChangesTrackerState> TrackerState { get; private set; }
            protected static Mock<IChangesCollector> ChangesCollector { get; private set; }
            protected static Mock<IEntityLinkBuilder> EntityLinkBuilder { get; private set; }
            protected static Mock<IEntityLinkFilter> EntityLinkFilter { get; private set; }

            protected static ChangesReferencesBuilder Target { get; private set; }
            protected static EntityLinksDataSource Result { get; private set; }
        }
    }
}