using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using Moq;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit
{
    public class IndexationAccTestFacade
    {
        private readonly EtlFlow _etlFlow;
        private readonly MockFinder _finder;
        readonly Mock<IOperationContextParser> _contextParser;
        readonly IChangesTrackerState _trackerState;

        public IndexationAccTestFacade(IDocsStorage docsStorage, IChangesTrackerState trackerState, params IQdsComponent[] components)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (trackerState == null)
            {
                throw new ArgumentNullException("trackerState");
            }

            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            _trackerState = trackerState;
            _contextParser = new Mock<IOperationContextParser>();

            var docModifiersRegistry = new DictionaryDocModifiersRegistry();
            var transformRelations = new TransformRelations();

            MetaBuilder(docModifiersRegistry, transformRelations, components);

            _finder = new MockFinder();

            var denTrans = new DenormalizerTransformation(new ErmEntitiesDenormalizer(new ErmDocsMetaData(docModifiersRegistry, transformRelations)));

            _etlFlow = new EtlFlow(new DocsPublisher(docsStorage), denTrans, new ErmExtractor(_finder), CreateReferencesBuilder(transformRelations));
        }

        IReferencesBuilder CreateReferencesBuilder(TransformRelations transformRelations)
        {
            return new ChangesReferencesBuilder(
                            new PboEntityLinkBuilder(_contextParser.Object),
                            new OperationsLogChangesCollector(_finder),
                            _trackerState,
                            new RelationsMetaEntityLinkFilter(transformRelations));
        }

        public long MockChangesForEntity<TEntity>(TEntity entity) where TEntity : class, IEntityKey, IEntity
        {
            var state = (RecordIdState)_trackerState.GetState();
            var newId = state.RecordId + 1;

            var operation = new PerformedBusinessOperation { Context = DateTime.Now.ToString(), Descriptor = DateTime.Now.Millisecond, Operation = DateTime.Now.Millisecond, Id = newId };
            _finder.Add(operation);

            var parsedOp = new Dictionary<StrictOperationIdentity, IEnumerable<long>>
                    {
                        { new StrictOperationIdentity(Mock.Of<IOperationIdentity>(), new EntitySet(typeof(TEntity).AsEntityName())), new[] { entity.Id } }
                    };

            _contextParser.Setup(p => p.GetGroupedIdsFromContext(operation.Context, operation.Operation, operation.Descriptor))
                .Returns(parsedOp);

            _finder.Add(entity);

            return newId;
        }

        public void ExecuteEtlFlow()
        {
            _etlFlow.Execute();
        }

        private static void MetaBuilder(DictionaryDocModifiersRegistry docModifiersRegistry,
                                            TransformRelations transformRelations,
                                            params IQdsComponent[] qdsComponents)
        {
            foreach (var qds in qdsComponents)
            {
                RegisterQdsComponent(docModifiersRegistry, transformRelations, qds);
            }
        }

        private static void RegisterQdsComponent(DictionaryDocModifiersRegistry docModifiersRegistry,
                                                     TransformRelations transformRelations,
                                                     IQdsComponent qdsComponent)
        {
            docModifiersRegistry.AddModifier(qdsComponent.CreateDocSelector());
            transformRelations.RegisterRelation(qdsComponent.PartsDocRelation);
        }
    }
}