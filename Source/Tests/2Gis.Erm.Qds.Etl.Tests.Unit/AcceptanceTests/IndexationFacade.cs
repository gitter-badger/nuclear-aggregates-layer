using System;
using System.Collections.Generic;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using Moq;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    public class IndexationFacade
    {
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;
        private readonly EtlFlow _etlFlow;
        private readonly MockFinder _finder;
        readonly Mock<IOldOperationContextParser> _contextParser;
        readonly IChangesTrackerState _trackerState;

        public IndexationFacade(IDocsStorage docsStorage, IQueryDsl queryDsl, IEntityToDocumentRelationMetadataContainer metadataContainer, IDocumentUpdaterFactory documentUpdaterFactory)
        {
            _metadataContainer = metadataContainer;

            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _trackerState = new DocsStorageChangesTrackerState(docsStorage);
            _contextParser = new Mock<IOldOperationContextParser>();

            _finder = new MockFinder();

            _etlFlow = new EtlFlow(new DocsPublisher(docsStorage, _trackerState), new DenormalizerTransformation(documentUpdaterFactory), new ErmExtractor(_finder), CreateReferencesBuilder(_metadataContainer));
        }

        public long LogChangesForEntity<TEntity>(TEntity entity)
            where TEntity : class, IEntityKey, IEntity
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

        public TDocument ExpectDocument<TEntity, TDocument>(TDocument document)
            where TDocument : class, IDoc, new()
        {
            var documentWrapper = new DocumentWrapper<TDocument> { Document = document };

            var mock2 = new Mock<IEntityToDocumentRelationMetadata>();
            mock2.SetupGet(x => x.DocumentType).Returns(typeof(TDocument));

            _metadataContainer.RegisterMetadataOverride<TEntity, TDocument>(() => mock2.Object);
            return document;
        }

        public void ExecuteEtlFlow()
        {
            _etlFlow.Execute();
        }

        IReferencesBuilder CreateReferencesBuilder(IEntityToDocumentRelationMetadataContainer metadataContainer)
        {
            return new ChangesReferencesBuilder(
                            new PboEntityLinkBuilder(_contextParser.Object),
                            new OperationsLogChangesCollector(_finder),
                            _trackerState,
                            new RelationsMetaEntityLinkFilter(metadataContainer));
        }
    }
}