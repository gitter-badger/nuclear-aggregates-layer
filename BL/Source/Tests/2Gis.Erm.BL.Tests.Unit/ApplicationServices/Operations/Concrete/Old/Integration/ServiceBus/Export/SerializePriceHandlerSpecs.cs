using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nuclear.Tracing.API;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.BL.Export
{
    public class SerializePriceHandlerSpecs
    {
        [Tags("BL")]
        [Subject(typeof(SerializePriceHandler))]
        public abstract class SerializePriceHandlerBaseContext
        {
            private Establish context = () =>
                {
                    RubCurrency = new Currency
                        {
                            Symbol = "RUB"
                        };

                    PublishedPrice = new Price
                        {
                            Id = 1,
                            IsPublished = true,
                            Currency = RubCurrency,
                            OrganizationUnit = new OrganizationUnit
                                {
                                    Projects = new[] { new Project { Id = 1 } }
                                }
                        };

                    PriceExportRepositoryMock = new Mock<IExportRepository<Price>>();

                    PriceExportRepositoryMock.Setup(x => x.GetEntityDtos(Moq.It.IsAny<IQueryBuilder<Price>>(),
                                                                         Moq.It.IsAny<ISelectSpecification<Price, IExportableEntityDto>>(),
                                                                         Moq.It.IsAny<IFindSpecification<Price>[]>()))
                                             .Returns(
                                                 (IQueryBuilder<Price> x, ISelectSpecification<Price, IExportableEntityDto> y, IFindSpecification<Price>[] z) =>
                                                 new[] { PublishedPrice }.AsQueryable().Select(y.Selector).ToArray());

                    SerializeRequest = SerializeObjectsRequest<Price, ExportFlowPriceListsPriceList>.Create(SchemaName, Enumerable.Empty<PerformedBusinessOperation>());

                    Handler = new SerializePriceHandler(PriceExportRepositoryMock.Object, Mock.Of<ICommonLog>());
                };

            private const string SchemaName = "flowPriceLists_PriceList";
            private static Currency RubCurrency { get; set; }
            private static Mock<IExportRepository<Price>> PriceExportRepositoryMock { get; set; }
            protected static SerializeObjectsResponse Response { get; set; }
            protected static Price PublishedPrice { get; set; }
            protected static SerializePriceHandler Handler { get; set; }
            protected static SerializeObjectsRequest<Price, ExportFlowPriceListsPriceList> SerializeRequest { get; set; }
        }

        private class When_serializing_correct_price : SerializePriceHandlerBaseContext
        {
            private static string _serializedPrice;

            private Establish context = () =>
                {
                    _serializedPrice = PublishedPrice.ToXml().ToString();
                };

            private Because of = () =>
                {
                    Response = (SerializeObjectsResponse)Handler.Handle(SerializeRequest);
                };

            private It should_have_no_failed_objects = () => Response.FailedObjects.Should().BeEmpty();
            private It should_have_serialized_price = () => Response.SerializedObjects.Should().Contain(_serializedPrice);
            private It should_have_single_success_object = () => Response.SuccessObjects.Should().HaveCount(1);
        }

        private class When_serializing_published_price_without_specified_project_code : SerializePriceHandlerBaseContext
        {
            private Establish context = () =>
                {
                    PublishedPrice.OrganizationUnit.Projects = new Project[0];
                };

            private Because of = () =>
                {  
                    Response = (SerializeObjectsResponse)Handler.Handle(SerializeRequest);
                };

            private It should_have_single_failed_object = () => Response.FailedObjects.Should().HaveCount(1);
            private It should_have_no_serialized_objects = () => Response.SerializedObjects.Should().BeEmpty();
            private It should_have_no_success_objects = () => Response.SuccessObjects.Should().BeEmpty();
        }

        private class When_serializing_published_price_without_specified_currency : SerializePriceHandlerBaseContext
        {
            private Establish context = () =>
                {
                    PublishedPrice.Currency.Symbol = null;
                };

            private Because of = () =>
            {
                Response = (SerializeObjectsResponse)Handler.Handle(SerializeRequest);
            };

            private It should_have_single_failed_object = () => Response.FailedObjects.Should().HaveCount(1);
            private It should_have_no_serialized_objects = () => Response.SerializedObjects.Should().BeEmpty();
            private It should_have_no_success_objects = () => Response.SuccessObjects.Should().BeEmpty();
        }

        private class When_serializing_published_price_with_unknown_currency_code : SerializePriceHandlerBaseContext
        {
            private Establish context = () =>
            {
                PublishedPrice.Currency.Symbol = string.Empty;
            };

            private Because of = () =>
            {
                Response = (SerializeObjectsResponse)Handler.Handle(SerializeRequest);
            };

            private It should_have_single_failed_object = () => Response.FailedObjects.Should().HaveCount(1);
            private It should_have_no_serialized_objects = () => Response.SerializedObjects.Should().BeEmpty();
            private It should_have_no_success_objects = () => Response.SuccessObjects.Should().BeEmpty();
        }
    }

    internal static class PriceExtensions
    {
        public static XElement ToXml(this Price price)
        {
            return new XElement("PriceList",
                                new XAttribute("Code", price.Id),
                                new XAttribute("PublishedDate", price.PublishDate),
                                new XAttribute("BeginingDate", price.BeginDate),
                                new XAttribute("IsPublished", price.IsPublished),
                                new XAttribute("BranchCode", price.OrganizationUnit.Projects.Select(x => x.Id).FirstOrDefault()),
                                new XAttribute("Currency", price.Currency.Symbol),
                                new XAttribute("IsHidden", !price.IsActive),
                                new XAttribute("IsDeleted", price.IsDeleted)
                );
        }
    }
}
