using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class DeleteClientLinkAggregateService : IDeleteClientLinkAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<ClientLink> _clientLinkRepository;
        private readonly IRepository<DenormalizedClientLink> _denormalizedClientLinksRepository;

        public DeleteClientLinkAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<ClientLink> clientLinkRepository,
            IRepository<DenormalizedClientLink> denormalizedClientLinksRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _clientLinkRepository = clientLinkRepository;
            _denormalizedClientLinksRepository = denormalizedClientLinksRepository;
        }

        public void Delete(ClientLink clientLink, IEnumerable<DenormalizedClientLink> currentGraph)
        {
            if (currentGraph.Select(x => x.GraphKey).Distinct().Count() > 2)
            {
                throw new ClientLinksDenormalizationException("ќбнаружена ошибка в данных разложени€ св€зей клиентов");
            }
            
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var linkToDelete = Tuple.Create(clientLink.MasterClientId, clientLink.ChildClientId);
                DeleteFromPrimaryStorage(clientLink);
                DeleteFromDenormalizatedStorage(linkToDelete, currentGraph);
                transaction.Complete();
            }
        }

        // TODO {y.baranihin, 28.08.2014}: ћожет быть имеет смысл разложить логику на набор методов c пон€тными названи€ми? ѕока вообще не пон€тно, что тут происходит
        private void DeleteFromDenormalizatedStorage(Tuple<long, long> linkToDelete, IEnumerable<DenormalizedClientLink> currentGraph)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<UpdateOrganizationStructureDenormalizationIdentity>())
            {
                var graph = currentGraph.Where(link => link.IsLinkedDirectly).Select(link => Tuple.Create(link.MasterClientId, link.ChildClientId)).ToList();
                graph.Remove(linkToDelete);
                var denormalization = graph.Denormalize();

                var deletedLinks = currentGraph.Where(link => !denormalization.Contains(Tuple.Create(link.MasterClientId, link.ChildClientId))).ToList();
                _denormalizedClientLinksRepository.DeleteRange(deletedLinks);
                operationScope.Deleted<DenormalizedClientLink>(deletedLinks);

                // ≈сли св€зь после удалени€ не исчезла, значит, она стала непр€мой
                if (denormalization.Contains(linkToDelete))
                {
                    var linkToUpdate = currentGraph.Single(link => link.MasterClientId == linkToDelete.Item1 && link.ChildClientId == linkToDelete.Item2);
                    linkToUpdate.IsLinkedDirectly = false;
                    _denormalizedClientLinksRepository.Update(linkToUpdate);
                    operationScope.Updated(linkToUpdate);
                }

                var graphsAfterLinkDeletion = graph.SplitGraphs().ToArray();
                if (graphsAfterLinkDeletion.Count() > 2)
                {
                    throw new ClientLinksDenormalizationException("Ќе могло так получитьс€, что в результате удалени€ одной св€зи граф распалс€ более, чем на два");
                }

                if (graphsAfterLinkDeletion.Count() == 2)
                {
                    var minimalGraph = graphsAfterLinkDeletion.OrderBy(nodes => nodes.Count()).First();
                    var linksOfMinimalGraph = currentGraph.Except(deletedLinks).Where(link => minimalGraph.Contains(link.MasterClientId));
                    var newGraphKey = Guid.NewGuid();
                    foreach (var link in linksOfMinimalGraph)
                    {
                        link.GraphKey = newGraphKey;
                        _denormalizedClientLinksRepository.Update(link);
                        operationScope.Updated(link);
                    }
                }

                _denormalizedClientLinksRepository.Save();
                operationScope.Complete();
            }
        }

        private void DeleteFromPrimaryStorage(ClientLink clientLink)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, ClientLink>())
            {
                _clientLinkRepository.Delete(clientLink);
                _clientLinkRepository.Save();
                operationScope.Deleted(clientLink);
                operationScope.Complete();
            }
        }
    }
}