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
        public class When_extract_by_entity_id : ErmExtractorContext
        {
            Establish context = () =>
            {
                _expectedId = 42;
                LinksDataReferences = CreateDataReferences(new EntityLink(EntityName.Client, _expectedId));

                long noiseId = 947;
                Finder.Setup(f => f.FindAll(Moq.It.Is<Type>(t => t == typeof(Client))))
                      .Returns(new[] { new Client { Id = _expectedId }, new Client { Id = noiseId } }.AsQueryable());

                Consumer.Setup(c => c.DataExtracted(Moq.It.IsAny<IData>()))
                        .Callback((IData data) => Data = (ErmData)data);
            };

            Because of = () => Target.Extract(LinksDataReferences, Consumer.Object);

            It should_query_entity_by_id = () => 
                Data.Data.Single().Entities.Cast<Client>().Single().Id.Should().Be(_expectedId);

            private static ErmData Data;
            private static int _expectedId;
        }

        [Subject(typeof(ErmExtractor))]
        public class When_extract_with_entity_name : ErmExtractorContext
        {
            Establish context = () =>
                {
                    LinksDataReferences = CreateDataReferences(new EntityLink(EntityName.Client, 42));

                    Consumer.Setup(c => c.DataExtracted(Moq.It.IsAny<IData>()))
                            .Callback((IData data) => Data = (ErmData)data);
                };

            Because of = () => Target.Extract(LinksDataReferences, Consumer.Object);

            It should_convert_entity_name_to_entity_type = () =>
                Data.Data.Single().EntityType.Should().Be<Client>();

            private static ErmData Data;
        }

        [Subject(typeof(ErmExtractor))]
        public class When_extract_data : ErmExtractorContext
        {
            Establish context = () =>
            {
                LinksDataReferences = CreateDataReferences(new EntityLink[0]);
            };

            Because of = () => Target.Extract(LinksDataReferences, Consumer.Object);

            It should_pass_erm_extracted_eata_to_counsumer = () =>
                 Consumer.Verify(c => c.DataExtracted(Moq.It.Is<IData>(d => d is ErmData)), Times.Once());
        }

        [Subject(typeof(ErmExtractor))]
        public class When_extract_by_entity_name : ErmExtractorContext
        {
            Establish context = () =>
            {
                LinksDataReferences = CreateDataReferences(
                    new EntityLink(EntityName.Client, 1),
                    new EntityLink(EntityName.Order, 2));

                Finder.Setup(f => f.FindAll(Moq.It.IsAny<Type>()))
                      .Returns((new IEntityKey[0]).AsQueryable());
            };

            Because of = () => Target.Extract(LinksDataReferences, Consumer.Object);

            It should_call_find_all_with_appropriate_entity_type = () =>
                {
                    VerifyFindAllOnceForEntity<Client>();
                    VerifyFindAllOnceForEntity<Order>();
                };

            static void VerifyFindAllOnceForEntity<T>()
            {
                Finder.Verify(f => f.FindAll(Moq.It.Is<Type>(t => t == typeof(T))), Times.Once(), "Ожидался вызов для типа сущности.");
            }
        }

        [Subject(typeof(ErmExtractor))]
        public class When_extract_called_for_not_erm_entity_data_references : ErmExtractorContext
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

        public class ErmExtractorContext
        {
            Establish context = () =>
            {
                Finder = new Mock<IFinder>();
                Consumer = new Mock<IDataConsumer>();
                Target = new ErmExtractor(Finder.Object);
            };

            public static Mock<IFinder> Finder { get; private set; }
            public static Mock<IDataConsumer> Consumer { get; private set; }
            public static ErmExtractor Target { get; private set; }
            public static EntityLinksDataSource LinksDataReferences { get; set; }

            public static EntityLinksDataSource CreateDataReferences(params EntityLink[] drs)
            {
                return new EntityLinksDataSource(drs, Mock.Of<ITrackState>());
            }
        }
    }
}