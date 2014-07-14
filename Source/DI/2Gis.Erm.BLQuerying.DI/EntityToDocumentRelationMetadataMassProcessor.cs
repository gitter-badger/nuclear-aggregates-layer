using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class EntityToDocumentRelationMetadataMassProcessor
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public EntityToDocumentRelationMetadataMassProcessor(IUnityContainer unityContainer, IEntityToDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public void MassProcess()
        {
            RegisterRelation(() => _unityContainer.Resolve<OrderToOrderGridDocRelation>());

            //RegisterRelation<Client, ClientGridDoc>(EntityToDocumentRelationMappings.SelectClientGridDoc);
            //RegisterRelation<Firm, FirmGridDoc>(EntityToDocumentRelationMappings.SelectFirmGridDoc);
            //RegisterRelation<Territory, TerritoryDoc>(EntityToDocumentRelationMappings.SelectTerritoryDoc);
            // last one
            //RegisterRelation<User, UserDoc>(() => _unityContainer.Resolve<UserToUserDocRelation>());
        }

        private void RegisterRelation<TEntity, TDocument>(Func<IQueryable<TEntity>, IEnumerable<IDocumentWrapper<TDocument>>> selectDocuments)
            where TEntity : class, IEntity, IEntityKey
        {
            Func<IEntityToDocumentRelation<TEntity, TDocument>> func = () =>
            {
                var relation = _unityContainer.Resolve<DefaultEntityToDocumentRelation<TEntity, TDocument>>();
                relation.SelectDocumentsFunc = selectDocuments;
                return relation;
            };

            RegisterRelation(func);
        }

        private void RegisterRelation<TEntity, TDocument>(Func<IEntityToDocumentRelation<TEntity, TDocument>> func)
        {
            _unityContainer.RegisterType<IEntityToDocumentRelation<TEntity, TDocument>>(new InjectionFactory(container => func()));
            _metadataContainer.RegisterMetadata<TEntity, TDocument>(() => new EntityToDocumentRelationMetadata<TEntity, TDocument>());
        }
    }
}
