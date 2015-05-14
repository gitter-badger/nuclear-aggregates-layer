using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Aggregates.Clients
{
    public class DeleteClientLinkAggregateServiceSpecs
    {
        public abstract class DeleteClientLinkAggregateServiceContext
        {
            protected static DeleteClientLinkAggregateService DeleteService;

            protected static ClientLink LinkToDelete;

            protected static List<DenormalizedClientLink> DenormalizedClientLinks;

            protected static List<DenormalizedClientLink> ExpectedLinks;

            protected static Guid GraphKey;

            private Establish context = () =>
                {
                    GraphKey = Guid.NewGuid();

                    var operationScopeFactory = Mock.Of<IOperationScopeFactory>();
                    var clientLinkRepository = Mock.Of<IRepository<ClientLink>>();
                    var denormalizedClientLinkRepository = Mock.Of<IRepository<DenormalizedClientLink>>();

                    Mock.Get(operationScopeFactory)
                        .Setup(x => x.CreateSpecificFor<DeleteIdentity, ClientLink>())
                        .Returns(Mock.Of<IOperationScope>);

                    Mock.Get(operationScopeFactory)
                        .Setup(x => x.CreateNonCoupled<UpdateOrganizationStructureDenormalizationIdentity>())
                        .Returns(Mock.Of<IOperationScope>);

                    Mock.Get(denormalizedClientLinkRepository)
                        .Setup(x => x.DeleteRange(Moq.It.IsAny<IEnumerable<DenormalizedClientLink>>()))
                        .Callback<IEnumerable<DenormalizedClientLink>>(x =>
                            {
                                foreach (var z in x)
                                {
                                    DenormalizedClientLinks.Remove(z);
                                }
                            });

                    Mock.Get(denormalizedClientLinkRepository)
                        .Setup(x => x.Delete(Moq.It.IsAny<DenormalizedClientLink>()))
                        .Callback<DenormalizedClientLink>(x => DenormalizedClientLinks.Remove(x));

                    Mock.Get(denormalizedClientLinkRepository)
                        .Setup(x => x.AddRange(Moq.It.IsAny<IEnumerable<DenormalizedClientLink>>()))
                        .Callback<IEnumerable<DenormalizedClientLink>>(x =>
                            {
                                foreach (var z in x)
                                {
                                    DenormalizedClientLinks.Add(z);
                                }
                            });

                    Mock.Get(denormalizedClientLinkRepository)
                        .Setup(x => x.Add(Moq.It.IsAny<DenormalizedClientLink>()))
                        .Callback<DenormalizedClientLink>(x => DenormalizedClientLinks.Add(x));


                    DeleteService = new DeleteClientLinkAggregateService(operationScopeFactory, clientLinkRepository, denormalizedClientLinkRepository);
                    DenormalizedClientLinks = new List<DenormalizedClientLink>();
                    ExpectedLinks = new List<DenormalizedClientLink>();
                };
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_single_link_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 1, ChildClientId = 2 };
                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_become_empty = () => DenormalizedClientLinks.Should().BeEmpty();
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_links_chain_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 2, ChildClientId = 3 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_links_chain_with_different_directions_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 1, ChildClientId = 2 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(3, 2, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_links_circle_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 3, ChildClientId = 1 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(2, 1, false, GraphKey)
                                           .AddDenormalizedLink(3, 1, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, false, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(2, 3, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_delete_link_in_graph : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
            {
                LinkToDelete = new ClientLink { MasterClientId = 1, ChildClientId = 2 };

                DenormalizedClientLinks.AddDenormalizedLink(1, 3, true, GraphKey)
                                       .AddDenormalizedLink(1, 2, true, GraphKey)
                                       .AddDenormalizedLink(1, 4, true, GraphKey)
                                       .AddDenormalizedLink(2, 3, true, GraphKey)
                                       .AddDenormalizedLink(2, 5, true, GraphKey)
                                       .AddDenormalizedLink(6, 4, true, GraphKey)
                                       .AddDenormalizedLink(1, 5, false, GraphKey);

                ExpectedLinks.AddDenormalizedLink(1, 3, true)
                             .AddDenormalizedLink(1, 4, true)
                             .AddDenormalizedLink(2, 5, true)
                             .AddDenormalizedLink(2, 3, true)
                             .AddDenormalizedLink(6, 4, true);
            };

            private Because of = () => 
                DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_remain_the_same_graph = () => DenormalizedClientLinks.Select(link => link.GraphKey).Distinct().Single().Should().Be(GraphKey);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_delete_link_with_graph_splitting : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
            {
                LinkToDelete = new ClientLink { MasterClientId = 1, ChildClientId = 2 };

                DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                       .AddDenormalizedLink(1, 4, true, GraphKey)
                                       .AddDenormalizedLink(2, 5, true, GraphKey)
                                       .AddDenormalizedLink(1, 5, false, GraphKey);

                ExpectedLinks.AddDenormalizedLink(1, 4, true)
                             .AddDenormalizedLink(2, 5, true);
            };

            private Because of = () =>
                DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_be_splitted_to_two_graphs = () => DenormalizedClientLinks.Select(link => link.GraphKey).Distinct().Count().Should().Be(2);
            private It should_contain_one_old_graph = () => DenormalizedClientLinks.Select(link => link.GraphKey).Distinct().Should().Contain(GraphKey);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_indirect_link_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 2, ChildClientId = 4 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(1, 4, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(3, 4, true, GraphKey)
                                           .AddDenormalizedLink(2, 4, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(1, 4, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(2, 4, false)
                                 .AddDenormalizedLink(3, 4, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks);

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_links_chain_with_different_directions_and_indirect_links_exists : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 2, ChildClientId = 3 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(3, 2, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_links_chain_with_different_directions_and_indirect_links_exists1 : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 2, ChildClientId = 4 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(1, 4, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(4, 3, true, GraphKey)
                                           .AddDenormalizedLink(2, 4, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(4, 3, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_tricky_configuration : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 3, ChildClientId = 1 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(2, 1, true, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, true, GraphKey)
                                           .AddDenormalizedLink(3, 1, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, false, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(2, 1, true)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(1, 3, true);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_another_tricky_configuration : DeleteClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToDelete = new ClientLink { MasterClientId = 2, ChildClientId = 3 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(2, 1, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, true, GraphKey)
                                           .AddDenormalizedLink(3, 1, true, GraphKey)

                                           .AddDenormalizedLink(1, 4, true, GraphKey)
                                           .AddDenormalizedLink(4, 1, true, GraphKey)
                                           .AddDenormalizedLink(2, 4, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, true, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(3, 4, true, GraphKey)
                                           .AddDenormalizedLink(4, 2, false, GraphKey)
                                           .AddDenormalizedLink(4, 3, false, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(2, 1, true)
                                 .AddDenormalizedLink(1, 3, true)
                                 .AddDenormalizedLink(3, 1, true)

                                 .AddDenormalizedLink(1, 4, true)
                                 .AddDenormalizedLink(4, 1, true)
                                 .AddDenormalizedLink(2, 4, true)
                                 .AddDenormalizedLink(3, 2, true)
                                 .AddDenormalizedLink(3, 4, true)
                                 .AddDenormalizedLink(2, 3, false)
                                 .AddDenormalizedLink(4, 2, false)
                                 .AddDenormalizedLink(4, 3, false);
                };

            private Because of = () => DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(DeleteClientLinkAggregateService))]
        public class When_different_configurations_are_considered : DeleteClientLinkAggregateServiceContext
        {
            private static IEnumerable<DenormalizedLinksTestData> TestData { get; set; }

            private Because of = () =>
                {
                    TestData = DenormalizedClientLinkTestsHelper.GetTestData(4, true);

                    foreach (var testData in TestData)
                    {
                        DenormalizedClientLinks = testData.DenormalizedClientLinks;
                        LinkToDelete = new ClientLink { MasterClientId = testData.MasterClientId, ChildClientId = testData.ChildClientId };
                        DeleteService.Delete(LinkToDelete, DenormalizedClientLinks.GetGraphState(LinkToDelete.MasterClientId, LinkToDelete.ChildClientId));
                        testData.Result = DenormalizedClientLinks;
                    }
                };

            private It should_have_proper_elements = () =>
                {
                    foreach (var data in TestData)
                    {
                        data.Result.ShouldBeEqualent(data.ExpectedLinks);
                    }
                };

            private It should_have_proper_graph_amounts = () =>
            {
                foreach (var data in TestData)
                {
                    var graphAmount = data.ExpectedLinks.GroupBy(x => x.GraphKey).Count();
                    data.Result.GroupBy(x => x.GraphKey).Count().Should().Be(graphAmount);
                }
            };
        }
    }
}