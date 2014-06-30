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

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    public class IndexationFacade
    {
        private readonly EtlFlow _etlFlow;
        private readonly MockFinder _finder;
        readonly Mock<IOperationContextParser> _contextParser;
        readonly IChangesTrackerState _trackerState;

        public IndexationFacade(IDocsStorage docsStorage, IQdsComponentsFactory qdsFactory, IQueryDsl queryDsl)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (qdsFactory == null)
            {
                throw new ArgumentNullException("qdsFactory");
            }
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _trackerState = new DocsStorageChangesTrackerState(docsStorage);
            _contextParser = new Mock<IOperationContextParser>();

            var docUpdatersRegistry = new DictionaryDocUpdatersRegistry();
            var transformRelations = new TransformRelations();

            _finder = new MockFinder();

            var denTrans = new DenormalizerTransformation(new ErmEntitiesDenormalizer(new ErmDocsMetaData(docUpdatersRegistry, transformRelations)),
                new MetadataBinder(docUpdatersRegistry, transformRelations), qdsFactory);

            _etlFlow = new EtlFlow(new DocsPublisher(docsStorage, _trackerState), denTrans, new ErmExtractor(_finder), CreateReferencesBuilder(transformRelations));
            _etlFlow.Init();
        }

        public long LogChangesForEntity<TEntity>(TEntity entity) where TEntity : class, IEntityKey, IEntity
        {
            var state = (RecordIdState)_trackerState.GetState();

            var recordIdLong = long.Parse(state.RecordId);
            var newId = recordIdLong + 1;

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

        IReferencesBuilder CreateReferencesBuilder(TransformRelations transformRelations)
        {
            return new ChangesReferencesBuilder(
                            new PboEntityLinkBuilder(_contextParser.Object),
                            new OperationsLogChangesCollector(_finder),
                            _trackerState,
                            new RelationsMetaEntityLinkFilter(transformRelations));
        }
    }
}