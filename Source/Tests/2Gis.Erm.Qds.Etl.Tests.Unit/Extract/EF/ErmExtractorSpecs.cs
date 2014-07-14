using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class ErmExtractorSpecs
    {
        [Subject(typeof(ErmExtractor))]
        class When_extract_by_entity_link : ErmExtractorContext
        {
            Establish context = () =>
                {
                    _expectedEntity = new Client { Id = 42 };
                    _expectedState = Mock.Of<ITrackState>();

                    Finder.Setup(f => f.FindAll(_expectedEntity.GetType()))
                        .Returns(new[] { _expectedEntity, new Client(), }.AsQueryable<IEntityKey>());

                    LinksDataReferences = CreateDataReferences(_expectedState, new EntityLink(EntityName.Client, _expectedEntity.Id));

                    Consumer.Setup(c => c.DataExtracted(Moq.It.IsAny<IData>()))
                            .Callback((IData data) => _extractedData = (ErmData)data);
                };

            Because of = () => Target.Extract(LinksDataReferences, Consumer.Object);

            It should_create_typed_entity_set = () => _extractedData.Data.Should().OnlyContain(set => set.EntityType == _expectedEntity.GetType());
            It should_only_contain_expected_entity = () => _extractedData.Data.Single().Entities.Should().OnlyContain(e => e == _expectedEntity);
            It should_pass_state_from_data_references = () => _extractedData.State.Should().Be(_expectedState);

            static ErmData _extractedData;
            static Client _expectedEntity;
            static ITrackState _expectedState;
        }

        [Subject(typeof(ErmExtractor))]
        class When_extract_called_for_not_erm_entity_data_references : ErmExtractorContext
        {
            Establish context = () =>
            {
                _unsupportedDataRefs = Mock.Of<IDataSource>();
            };

            Because of = () => _actualException = Catch.Exception(() =>
                Target.Extract(_unsupportedDataRefs, Consumer.Object));

            It should_fail = () =>
                _actualException.Should()
                .NotBeNull("Ожидалось исключение.")
                .And
                .BeOfType<ArgumentException>("Не верный тип исключения.");

            private static Exception _actualException;
            private static IDataSource _unsupportedDataRefs;
        }

        class ErmExtractorContext
        {
            Establish context = () =>
            {
                Finder = new Mock<IFinder>();
                Consumer = new Mock<IDataConsumer>();
                Target = new ErmExtractor(Finder.Object);
            };

            protected static Mock<IFinder> Finder { get; private set; }
            protected static Mock<IDataConsumer> Consumer { get; set; }
            protected static ErmExtractor Target { get; set; }
            protected static EntityLinksDataSource LinksDataReferences { get; set; }

            protected static EntityLinksDataSource CreateDataReferences(ITrackState state, params EntityLink[] drs)
            {
                return new EntityLinksDataSource(drs, state);
            }
        }
    }
}