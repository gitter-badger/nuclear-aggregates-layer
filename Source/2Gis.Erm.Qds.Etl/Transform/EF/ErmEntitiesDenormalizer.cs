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

        public void DenormalizeByType(Type entityType, IEnumerable<IEntityKey> entities)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var array = entities.ToArray();

            foreach (var modifier in _docsModifierRegistry.GetDocsUpdaters(entityType))
            {
                _list.AddRange(modifier.UpdateDocuments(array));
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