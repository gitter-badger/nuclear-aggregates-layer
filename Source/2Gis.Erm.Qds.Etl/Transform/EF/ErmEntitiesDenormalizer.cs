using System;
using System.Collections;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

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

            foreach (var modifier in _docsModifierRegistry.GetDocsSelectors(entityType))
            {
                _list.AddRange(modifier.ModifyDocuments(entities));
            }
        }

        public IEnumerable<IDoc> GetChangedDocuments()
        {
            return _list;
        }
    }
}