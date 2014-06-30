using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class ErmEntitiesDenormalizer
    {
        private readonly IDocsMetaData _docsModifierRegistry;
        private readonly List<IDoc> _list = new List<IDoc>();

        public ErmEntitiesDenormalizer(IDocsMetaData docsModifierRegistry)
        {
            if (docsModifierRegistry == null)
            {
                throw new ArgumentNullException("docsModifierRegistry");
            }
            _docsModifierRegistry = docsModifierRegistry;
        }

        public void DenormalizeByType(Type entityType, IQueryable<IEntityKey> entities)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var updaters = _docsModifierRegistry.GetDocsUpdaters(entityType);
            foreach (var updater in updaters)
            {
                var documents = updater.UpdateDocuments(entities);
                _list.AddRange(documents);
            }
        }

        public IEnumerable<IDoc> GetChangedDocuments()
        {
            return _list;
        }

        public void ClearChangedDocuments()
        {
            _list.Clear();
        }
    }
}