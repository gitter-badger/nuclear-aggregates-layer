using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class UnityDocumentRelationsRegistry : IDocumentRelationsRegistry
    {
        private readonly IUnityContainer _container;
        private readonly Dictionary<Type, List<Type>> _docToPartsMap = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, List<Type>> _partToDocsMap = new Dictionary<Type, List<Type>>();

        public UnityDocumentRelationsRegistry(IUnityContainer container)
        {
            _container = container;
        }

        // FIXME {m.pashuk, 29.04.2014}: Я бы поговорил. Вангую, что класс разрастется. Плюс смешение логики: описываем метаданные, наполняем и еще и применяем для изменения документов.
        public UnityDocumentRelationsRegistry RegisterAllDocumentParts(Func<LifetimeManager> lifetime)
        {
            RegisterPart<ClientGridDoc, UserDoc>(r => r
                .PartId(x => x.OwnerCode)
                .InsertPart((clientGridDoc, userDoc) =>
                {
                    clientGridDoc.OwnerName = userDoc.Name;
                    InsertAuthorizationPart(clientGridDoc, userDoc);
                })
            , lifetime);

            RegisterPart<ClientGridDoc, TerritoryDoc>(r => r
                .PartId(x => x.TerritoryId)
                .InsertPart((clientGridDoc, territoryDoc) =>
                {
                    clientGridDoc.TerritoryName = territoryDoc.Name;
                })
            , lifetime);

            return this;
        }

        private void RegisterPart<TDocument, TDocumentPart>(Action<DocumentRelation<TDocument, TDocumentPart>> action, Func<LifetimeManager> lifetime)
            where TDocument : class
            where TDocumentPart : class, IDoc
        {
            _container.RegisterType<DocumentRelation<TDocument, TDocumentPart>>(lifetime(), new InjectionFactory(x =>
            {
                var documentRelation = new DocumentRelation<TDocument, TDocumentPart>(_container.Resolve<IElasticApi>());
                action(documentRelation);
                return documentRelation;
            }));

            AddToMap(_docToPartsMap, typeof(TDocument), typeof(TDocumentPart));
            AddToMap(_partToDocsMap, typeof(TDocumentPart), typeof(TDocument));
        }

        private static void AddToMap(Dictionary<Type, List<Type>> map, Type key, Type value)
        {
            List<Type> types;
            if (!map.TryGetValue(key, out types))
            {
                types = new List<Type>();
                map.Add(key, types);
            }

            types.Add(value);
        }

        public bool TryGetDocumentRelations<TDocument>(out IEnumerable<IDocumentRelation<TDocument>> relations)
        {
            var key = typeof(TDocument);

            List<Type> documentTypes;
            if (!_docToPartsMap.TryGetValue(key, out documentTypes))
            {
                relations = null;
                return false;
            }

            relations = documentTypes.Select(x =>
            {
                var relationType = typeof(DocumentRelation<,>).MakeGenericType(typeof(TDocument), x);
                return (IDocumentRelation<TDocument>)_container.Resolve(relationType);
            });

            return true;
        }

        public bool TryGetDocumentPartRelations<TDocumentPart>(out IEnumerable<IDocumentPartRelation<TDocumentPart>> relations)
        {
            var key = typeof(TDocumentPart);

            List<Type> documentTypes;
            if (!_partToDocsMap.TryGetValue(key, out documentTypes))
            {
                relations = null;
                return false;
            }

            relations = documentTypes.Select(x =>
            {
                var relationType = typeof(DocumentRelation<,>).MakeGenericType(x, typeof(TDocumentPart));
                return (IDocumentPartRelation<TDocumentPart>)_container.Resolve(relationType);
            });

            return true;
        }

        private static void InsertAuthorizationPart(IAuthorizationDoc authorizationDoc, UserDoc userDoc)
        {
            if (userDoc.Permissions == null)
            {
                return;
            }

            var documentTypeName = authorizationDoc.GetType().Name;
            var tags = userDoc.Permissions
                              .Where(x => x.Operation.IndexOf(documentTypeName, StringComparison.OrdinalIgnoreCase) > -1 && x.Tags != null)
                              .SelectMany(x => x.Tags.Concat(new[] { x.Operation })).Distinct();

            authorizationDoc.Authorization = new DocumentAuthorization { Tags = tags };
        }
    }
}
