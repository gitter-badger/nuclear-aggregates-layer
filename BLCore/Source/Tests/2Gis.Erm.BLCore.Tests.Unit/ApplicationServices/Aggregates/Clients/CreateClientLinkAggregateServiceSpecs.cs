using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
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
    public class CreateClientLinkAggregateServiceSpecs
    {
        public abstract class CreateClientLinkAggregateServiceContext
        {
            protected static CreateClientLinkAggregateService CreateService;

            protected static ClientLink LinkToAdd;

            protected static List<DenormalizedClientLink> DenormalizedClientLinks;

            protected static List<DenormalizedClientLink> ExpectedLinks;

            protected static Guid GraphKey;

            private Establish context = () =>
                {
                    var operationScopeFactory = Mock.Of<IOperationScopeFactory>();
                    var clientLinkRepository = Mock.Of<IRepository<ClientLink>>();
                    var denormalizedClientLinkRepository = Mock.Of<IRepository<DenormalizedClientLink>>();
                    var identityProvider = Mock.Of<IIdentityProvider>();

                    GraphKey = Guid.NewGuid();

                    Mock.Get(operationScopeFactory)
                        .Setup(x => x.CreateSpecificFor<CreateIdentity, ClientLink>())
                        .Returns(Mock.Of<IOperationScope>);

                    Mock.Get(operationScopeFactory)
                        .Setup(x => x.CreateNonCoupled<UpdateOrganizationStructureDenormalizationIdentity>())
                        .Returns(Mock.Of<IOperationScope>);

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

                    Mock.Get(identityProvider)
                        .Setup(x => x.SetFor(Moq.It.IsAny<DenormalizedClientLink[]>()))
                        .Callback<IEnumerable<DenormalizedClientLink>>(x =>
                            {
                                foreach (var z in x)
                                {
                                    z.Id = z.MasterClientId * 1000 + z.ChildClientId;
                                }
                            });

                    CreateService = new CreateClientLinkAggregateService(operationScopeFactory, clientLinkRepository, identityProvider, denormalizedClientLinkRepository);
                    DenormalizedClientLinks = new List<DenormalizedClientLink>();
                    ExpectedLinks = new List<DenormalizedClientLink>();
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_add_single_link : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 1, ChildClientId = 2 };
                    DenormalizedClientLinks = new List<DenormalizedClientLink>();
                    ExpectedLinks = new List<DenormalizedClientLink>
                        {
                            new DenormalizedClientLink
                                {
                                    Id = 1,
                                    MasterClientId = 1,
                                    ChildClientId = 2,
                                    IsLinkedDirectly = true
                                },
                        };
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_single_record = () => DenormalizedClientLinks.Should().HaveCount(1);
            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_add_link_to_make_chain : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 2, ChildClientId = 3 };
                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(2, 3, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class add_link_to_make_chain_with_different_directions_exists : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 1, ChildClientId = 2 };

                    DenormalizedClientLinks.AddDenormalizedLink(3, 2, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(3, 2, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_links_chain_with_different_directions_and_indirect_links_exists : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 2, ChildClientId = 3 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(3, 2, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(3, 2, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_links_chain_with_different_directions_and_indirect_links_exists1 : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 2, ChildClientId = 4 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(4, 3, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(1, 4, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(4, 3, true)
                                 .AddDenormalizedLink(2, 4, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_add_link_link_to_make_circle : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 3, ChildClientId = 1 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(2, 1, false)
                                 .AddDenormalizedLink(3, 1, true)
                                 .AddDenormalizedLink(3, 2, false);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_add_link_to_make_direct_link : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 2, ChildClientId = 4 };

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(1, 4, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(2, 4, false, GraphKey)
                                           .AddDenormalizedLink(3, 4, true, GraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(1, 4, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(3, 4, true)
                                 .AddDenormalizedLink(2, 4, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_add_link_to_connect_two_graphs : CreateClientLinkAggregateServiceContext
        {
            private Establish context = () =>
                {
                    LinkToAdd = new ClientLink { MasterClientId = 3, ChildClientId = 4 };

                    var secondGraphKey = Guid.NewGuid();

                    DenormalizedClientLinks.AddDenormalizedLink(1, 2, true, GraphKey)
                                           .AddDenormalizedLink(1, 3, false, GraphKey)
                                           .AddDenormalizedLink(2, 3, true, GraphKey)
                                           .AddDenormalizedLink(4, 5, true, secondGraphKey);

                    ExpectedLinks.AddDenormalizedLink(1, 2, true)
                                 .AddDenormalizedLink(1, 3, false)
                                 .AddDenormalizedLink(1, 4, false)
                                 .AddDenormalizedLink(1, 5, false)
                                 .AddDenormalizedLink(2, 3, true)
                                 .AddDenormalizedLink(2, 4, false)
                                 .AddDenormalizedLink(2, 5, false)
                                 .AddDenormalizedLink(3, 4, true)
                                 .AddDenormalizedLink(3, 5, false)
                                 .AddDenormalizedLink(4, 5, true);
                };

            private Because of = () => CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));

            private It should_have_proper_amount_of_elements = () => DenormalizedClientLinks.Should().HaveCount(ExpectedLinks.Count);

            private It should_have_proper_elements = () => DenormalizedClientLinks.ShouldBeEqualent(ExpectedLinks);

            private It should_have_proper_graphKey = () =>
                {
                    foreach (var link in DenormalizedClientLinks)
                    {
                        link.GraphKey.Should().Be(GraphKey);
                    }
                };
        }

        [Subject(typeof(CreateClientLinkAggregateService))]
        public class When_different_configurations_are_considered : CreateClientLinkAggregateServiceContext
        {
            private static IEnumerable<DenormalizedLinksTestData> TestData { get; set; }

            private Because of = () =>
                {
                    TestData = DenormalizedClientLinkTestsHelper.GetTestData(4, false);

                    foreach (var testData in TestData)
                    {
                        DenormalizedClientLinks = testData.DenormalizedClientLinks;
                        LinkToAdd = new ClientLink { MasterClientId = testData.MasterClientId, ChildClientId = testData.ChildClientId };
                        CreateService.Create(LinkToAdd, DenormalizedClientLinks.GetGraphState(LinkToAdd.MasterClientId, LinkToAdd.ChildClientId));
                        testData.Result = DenormalizedClientLinks;
                    }
                };

            private It should_have_elements = () =>
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