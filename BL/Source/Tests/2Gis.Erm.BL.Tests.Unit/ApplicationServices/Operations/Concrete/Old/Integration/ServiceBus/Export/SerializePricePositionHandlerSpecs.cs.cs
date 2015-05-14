using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.BL.Export
{
    public class SerializePricePositionHandlerSpecs
    {
        [Tags("BL")]
        [Subject(typeof(SerializePricePositionHandler))]
        public abstract class SerializePricePositionHandlerBaseContext
        {
            private Establish context = () =>
            {
                PricePositionExportRepositoryMock = new Mock<IExportRepository<PricePosition>>();

                TestPricePosition = new PricePosition
                {
                    Id = 1,
                    PriceId = 2,
                    Cost = 100M,
                    PositionId = 3
                };

                PricePositionExportRepositoryMock.Setup(x => x.GetEntityDtos(Moq.It.IsAny<IQueryBuilder<PricePosition>>(),
                                                                             Moq.It.IsAny<SelectSpecification<PricePosition, IExportableEntityDto>>(),
                                                                             Moq.It.IsAny<FindSpecification<PricePosition>[]>()))
                                                 .Returns(
                                                     (IQueryBuilder<Price> x,
                                                      SelectSpecification<PricePosition, IExportableEntityDto> y,
                                                      FindSpecification<PricePosition>[] z) =>
                                                     new[] { TestPricePosition }.AsQueryable().Select(y.Selector).ToArray());

                SerializeRequest = SerializeObjectsRequest<PricePosition, ExportFlowPriceListsPriceListPosition>.Create(SchemaName, Enumerable.Empty<PerformedBusinessOperation>());

                Handler = new SerializePricePositionHandler(PricePositionExportRepositoryMock.Object, Mock.Of<ITracer>());
            };

            private const string SchemaName = "flowPriceLists_PriceListPosition";
            private static Mock<IExportRepository<PricePosition>> PricePositionExportRepositoryMock { get; set; }
            protected static SerializeObjectsResponse Response { get; set; }
            protected static PricePosition TestPricePosition { get; set; }
            protected static SerializePricePositionHandler Handler { get; set; }
            protected static SerializeObjectsRequest<PricePosition, ExportFlowPriceListsPriceListPosition> SerializeRequest { get; set; }
        }

        class When_serializing_correct_price_position : SerializePricePositionHandlerBaseContext
        {
            private static string _serializedPricePosition;

            private Establish context = () =>
            {
                _serializedPricePosition = TestPricePosition.ToXml().ToString();
            };

            private Because of = () =>
            {
                Response = (SerializeObjectsResponse)Handler.Handle(SerializeRequest);
            };

            private It should_have_no_failed_objects = () => Response.FailedObjects.Should().BeEmpty();
            private It should_have_serialized_price = () => Response.SerializedObjects.Should().Contain(_serializedPricePosition);
            private It should_have_single_success_object = () => Response.SuccessObjects.Should().HaveCount(1);
        }
    }

    internal static class PricePositionExtensions
    {
        public static XElement ToXml(this PricePosition pricePosition)
        {
            return new XElement("PriceListPosition",
                                new XAttribute("Code", pricePosition.Id),
                                new XAttribute("PriceListCode", pricePosition.PriceId),
                                new XAttribute("NomenclatureCode", pricePosition.PositionId),
                                new XAttribute("Cost", pricePosition.Cost),
                                new XAttribute("IsHidden", !pricePosition.IsActive)
                );
        }
    }
}
