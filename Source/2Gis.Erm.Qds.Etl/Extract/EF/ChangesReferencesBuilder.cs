using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class ChangesReferencesBuilder : IReferencesBuilder
    {
        private readonly IEntityLinkBuilder _entityLinkBuilder;
        private readonly IChangesTrackerState _trackerState;
        private readonly IChangesCollector _changesCollector;
        private readonly IEntityLinkFilter _entityLinkFilter;

        public ChangesReferencesBuilder(IEntityLinkBuilder entityLinkBuilder,
                                IChangesCollector changesCollector,
                                IChangesTrackerState trackerState,
                                IEntityLinkFilter entityLinkFilter)
        {
            if (entityLinkBuilder == null)
            {
                throw new ArgumentNullException("entityLinkBuilder");
            }

            if (changesCollector == null)
            {
                throw new ArgumentNullException("changesCollector");
            }

            if (trackerState == null)
            {
                throw new ArgumentNullException("trackerState");
            }

            if (entityLinkFilter == null)
            {
                throw new ArgumentNullException("entityLinkFilter");
            }

            _entityLinkBuilder = entityLinkBuilder;
            _changesCollector = changesCollector;
            _trackerState = trackerState;
            _entityLinkFilter = entityLinkFilter;
        }

        public void BuildReferences(IReferencesConsumer referencesConsumer)
        {
            if (referencesConsumer == null)
            {
                throw new ArgumentNullException("referencesConsumer");
            }

            var state = _trackerState.GetState();

            var changes = _changesCollector.LoadChanges(state);

            referencesConsumer.ReferencesBuilt(new EntityLinksDataSource(TrackChanges(changes), _changesCollector.CurrentState));
        }

        IEnumerable<EntityLink> TrackChanges(IEnumerable<IChangeDescriptor> changes)
        {
            return changes.SelectMany(descriptor =>
            {
                var links = _entityLinkBuilder.CreateEntityLinks(descriptor);
                return links.Where(link => _entityLinkFilter.IsSupported(link));
            });
        }
    }
}