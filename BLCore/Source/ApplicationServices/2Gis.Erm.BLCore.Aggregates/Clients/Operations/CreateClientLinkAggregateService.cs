using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class CreateClientLinkAggregateService : ICreateClientLinkAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<ClientLink> _clientLinkRepository;
        private readonly IRepository<DenormalizedClientLink> _denormalizedClientLinksRepository;
        private readonly IIdentityProvider _identityProvider;

        public CreateClientLinkAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<ClientLink> clientLinkRepository,
            IIdentityProvider identityProvider,
            IRepository<DenormalizedClientLink> denormalizedClientLinksRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _clientLinkRepository = clientLinkRepository;
            _identityProvider = identityProvider;
            _denormalizedClientLinksRepository = denormalizedClientLinksRepository;
        }

        public void Create(ClientLink clientLink, IEnumerable<DenormalizedClientLink> currentGraph)
        {
            if (currentGraph.Select(x => x.GraphKey).Distinct().Count() > 2)
            {
                throw new ClientLinksDenormalizationException("ќбнаружена ошибка в данных разложени€ св€зей клиентов");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                CreateInPrimaryStorage(clientLink);
                
                var newLink = Tuple.Create(clientLink.MasterClientId, clientLink.ChildClientId);
                CreateInDenormalizatedStorage(newLink, currentGraph);
                
                transaction.Complete();
            }
        }

        private static Guid GetGraphKey(IEnumerable<DenormalizedClientLink> currentGraph)
        {
            var graphKey = currentGraph.GroupBy(x => x.GraphKey).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault();
            if (graphKey == default(Guid))
            {
                graphKey = Guid.NewGuid();
            }

            return graphKey;
        }

        private static Tuple<long, long>[] EvaluateNewLinks(Tuple<long, long> newClientLink, IEnumerable<DenormalizedClientLink> currentGraph)
        {
            var graph = currentGraph.Where(link => link.IsLinkedDirectly).Select(link => Tuple.Create(link.MasterClientId, link.ChildClientId)).ToList();
            graph.Add(newClientLink);
            var denormalization = graph.Denormalize();

            var newLinks = denormalization.Except(currentGraph.Select(link => Tuple.Create(link.MasterClientId, link.ChildClientId)))
                                          .ToArray();
            return newLinks;
        }

        private void CreateInPrimaryStorage(ClientLink newClientLink)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, ClientLink>())
            {
                _identityProvider.SetFor(newClientLink);
                _clientLinkRepository.Add(newClientLink);
                operationScope.Added(newClientLink);
                
                _clientLinkRepository.Save();
                operationScope.Complete();
            }
        }
        
        private void CreateInDenormalizatedStorage(Tuple<long, long> newClientLink, IEnumerable<DenormalizedClientLink> currentGraph)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<UpdateOrganizationStructureDenormalizationIdentity>())
            {
                var graphKey = GetGraphKey(currentGraph);
                var newLinks = EvaluateNewLinks(newClientLink, currentGraph);

                var newLinkEntities = newLinks.Select(link => new DenormalizedClientLink
                                                                {
                                                                    MasterClientId = link.Item1,
                                                                    ChildClientId = link.Item2,
                                                                    GraphKey = graphKey,
                                                                    IsLinkedDirectly = link.Equals(newClientLink)
                                                                })
                                              .ToArray();
                _identityProvider.SetFor(newLinkEntities);
                _denormalizedClientLinksRepository.AddRange(newLinkEntities);
                operationScope.Added<DenormalizedClientLink>(newLinkEntities);

                // ≈сли св€зи не оказалось в числе созданных - значит, она была и ранее, но косвенна€
                if (!newLinks.Contains(newClientLink))
                {
                    var entityToUpdate = currentGraph.Single(x => x.MasterClientId == newClientLink.Item1 && x.ChildClientId == newClientLink.Item2);
                    entityToUpdate.IsLinkedDirectly = true;
                    _denormalizedClientLinksRepository.Update(entityToUpdate);
                    operationScope.Updated(entityToUpdate);
                }

                var mergedGraph = currentGraph.Where(link => link.GraphKey != graphKey).ToArray();
                if (mergedGraph.Any())
                {
                    foreach (var link in mergedGraph)
                    {
                        link.GraphKey = graphKey;
                        _denormalizedClientLinksRepository.Update(link);
                    }

                    operationScope.Updated<DenormalizedClientLink>(mergedGraph);
                }

                _denormalizedClientLinksRepository.Save();

                operationScope.Complete();
            }
        }
    }
}