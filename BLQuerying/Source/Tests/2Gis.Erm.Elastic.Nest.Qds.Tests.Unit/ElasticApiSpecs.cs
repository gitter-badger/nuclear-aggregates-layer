using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Settings;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    // FIXME {m.pashuk, 14.05.2014}: обернуть в класс контейнер ElasticApiSpecs

    public class When_scroll_on_empty_data : ElasticApiContext
    {
        Establish context = () =>
        {
            // FIXME {m.pashuk, 14.05.2014}: Дублирование логики создания мока ISearchResponse. Можно сделать метод в базовом классе теста.
            var searchResponseMock = new Mock<ISearchResponse<ElasticApiTestDoc>>();
            searchResponseMock.Setup(x => x.Total).Returns(0);
            searchResponseMock.Setup(x => x.ScrollId).Returns(Guid.NewGuid().ToString());

            ElasticClientMock.Setup(x => x.Search(Moq.It.IsAny<Func<SearchDescriptor<ElasticApiTestDoc>, SearchDescriptor<ElasticApiTestDoc>>>())).Returns(searchResponseMock.Object);
        };

        Because of = () => Documents = ElasticApi.Scroll<ElasticApiTestDoc>(x => x);

        It documents_empty = () => Documents.Should().BeEmpty();

        private static IEnumerable<IDocumentWrapper<ElasticApiTestDoc>> Documents { get; set; }
    }

    public class When_scroll_on_not_empty_data : ElasticApiContext
    {
        Establish context = () =>
        {
            SampleData = new[] { new Mock<IHit<ElasticApiTestDoc>>().Object, new Mock<IHit<ElasticApiTestDoc>>().Object };

            // FIXME {m.pashuk, 14.05.2014}: Дублирование логики создания мока ISearchResponse. Можно сделать метод в базовом классе теста.
            var searchResponseMock = new Mock<ISearchResponse<ElasticApiTestDoc>>();
            searchResponseMock.Setup(x => x.Total).Returns(SampleData.Count());
            searchResponseMock.Setup(x => x.ScrollId).Returns(Guid.NewGuid().ToString());

            ElasticClientMock.Setup(x => x.Search(Moq.It.IsAny<Func<SearchDescriptor<ElasticApiTestDoc>, SearchDescriptor<ElasticApiTestDoc>>>())).Returns(searchResponseMock.Object);

            // FIXME {m.pashuk, 14.05.2014}: Дублирование логики создания мока ISearchResponse. Можно сделать метод в базовом классе теста.
            var scrollResponseMock1 = new Mock<ISearchResponse<ElasticApiTestDoc>>();
            scrollResponseMock1.Setup(x => x.Hits).Returns(SampleData);
            scrollResponseMock1.Setup(x => x.ScrollId).Returns(Guid.NewGuid().ToString());

            // FIXME {m.pashuk, 14.05.2014}: Дублирование логики создания мока ISearchResponse. Можно сделать метод в базовом классе теста.
            var scrollResponseMock2 = new Mock<ISearchResponse<ElasticApiTestDoc>>();
            scrollResponseMock2.Setup(x => x.Hits).Returns(Enumerable.Empty<IHit<ElasticApiTestDoc>>());
            scrollResponseMock2.Setup(x => x.ScrollId).Returns((string)null);

            var queue = new Queue<ISearchResponse<ElasticApiTestDoc>>();
            queue.Enqueue(scrollResponseMock1.Object);
            queue.Enqueue(scrollResponseMock2.Object);
            ElasticClientMock.Setup(x => x.Scroll(Moq.It.IsAny<Func<ScrollDescriptor<ElasticApiTestDoc>, ScrollDescriptor<ElasticApiTestDoc>>>())).Returns(queue.Dequeue);
        };

        Because of = () => Documents = ElasticApi.Scroll<ElasticApiTestDoc>(x => x).ToArray();

        It returns_sample_data_count = () => Documents.Should().HaveSameCount(SampleData);
        It returns_sample_data = () => Documents.Should().BeEquivalentTo(SampleData);

        private static IEnumerable<IHit<ElasticApiTestDoc>> SampleData { get; set; }
        private static IEnumerable<IDocumentWrapper<ElasticApiTestDoc>> Documents { get; set; }
    }

    [Subject(typeof(ElasticApi))]
    public class ElasticApiContext
    {
        Establish context = () =>
        {
            ElasticClientMock = new Mock<IElasticClient>();
            var nestSettingsMock = new Mock<INestSettings>();

            // TODO {m.pashuk, 14.05.2014}: Давай тестирумый мембер называть Target?
            ElasticApi = new ElasticApi(ElasticClientMock.Object, nestSettingsMock.Object, null);
        };

        protected static Mock<IElasticClient> ElasticClientMock { get; private set; }
        protected static IElasticApi ElasticApi { get; private set; }
    }

    public sealed class ElasticApiTestDoc { }
}
