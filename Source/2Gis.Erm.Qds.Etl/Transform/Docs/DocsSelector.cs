using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class DocsSelector<TDoc> : IDocsSelector where TDoc : class, IDoc
    {
        public DocsSelector(IQdsComponent qdsComponent, IRelationalDocsFinder relationalDocsFinder, IDocMapper<TDoc> mapper)
        {
            if (relationalDocsFinder == null)
            {
                throw new ArgumentNullException("relationalDocsFinder");
            }

            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }

            QdsComponent = qdsComponent;
            RelationalDocsFinder = relationalDocsFinder;
            Mapper = mapper;
        }

        public virtual IEnumerable<IDoc> ModifyDocuments(IEnumerable<IEntityKey> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            foreach (var entity in entities)
            {
                var docs = RelationalDocsFinder.FindDocsByRelatedPart<TDoc>(entity);

                if (!docs.Any())
                {
                    var newDoc = (TDoc)QdsComponent.CreateNewDoc(entity);
                    if (newDoc != null)
                        docs = new[] { newDoc };
                }

                Mapper.UpdateDocByEntity(docs, entity);

                foreach (var doc in docs)
                    yield return doc;
            }
        }

        public IQdsComponent QdsComponent { get; private set; }
        public IRelationalDocsFinder RelationalDocsFinder { get; private set; }
        public IDocMapper<TDoc> Mapper { get; private set; }

        public Type SupportedDocType
        {
            get
            {
                return typeof(TDoc);
            }
        }
    }
}