using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class DocumentRelationMetadataMassProcessor
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDocumentRelationMetadataContainer _metadataContainer;

        public DocumentRelationMetadataMassProcessor(IUnityContainer unityContainer, IDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public void MassProcess()
        {
            RegisterMetadata<ClientGridDoc, TerritoryDoc>(x => x.TerritoryId, DocumentRelationMappings.InsertTerritoryDocToClientGridDoc);
            RegisterMetadata<ClientGridDoc, UserDoc>(x => x.OwnerCode, DocumentRelationMappings.InsertUserDocToClientGridDoc);
            RegisterMetadata<ClientGridDoc, FirmGridDoc>(x => x.MainFirmId, DocumentRelationMappings.InsertFirmGridDocToClientGridDoc);

            RegisterMetadata<FirmGridDoc, TerritoryDoc>(x => x.TerritoryId, DocumentRelationMappings.InsertTerritoryDocToFirmGridDoc);
            RegisterMetadata<FirmGridDoc, ClientGridDoc>(x => x.ClientId, DocumentRelationMappings.InsertClientGridDocToFirmGridDoc);
            RegisterMetadata<FirmGridDoc, UserDoc>(x => x.OwnerCode, DocumentRelationMappings.InsertUserDocToFirmGridDoc);
        }

        private void RegisterMetadata<TDocument, TDocumentPart>(Expression<Func<TDocument, string>> documentPartIdExpression, Action<TDocument, TDocumentPart> insertDocumentPartFunc)
        {
            _metadataContainer.RegisterMetadata(() =>
                {
                    var metadata = _unityContainer.Resolve<DefaultDocumentRelationMetadata<TDocument, TDocumentPart>>();
                    metadata.SetDocumentPartId(documentPartIdExpression);
                    metadata.InsertDocumentPartFunc = insertDocumentPartFunc;

                    return metadata;
                });
        }
    }
}