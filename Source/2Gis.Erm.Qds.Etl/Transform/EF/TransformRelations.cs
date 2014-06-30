using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class TransformRelations : ITransformRelations
    {
        private readonly Dictionary<Type, List<Type>> _dictionary = new Dictionary<Type, List<Type>>();

        public bool TryGetRelatedDocTypes(Type entityType, out IEnumerable<Type> docTypes)
        {
            List<Type> docTypesList;
            if (_dictionary.TryGetValue(entityType, out docTypesList))
            {
                docTypes = docTypesList;
                return true;
            }

            docTypes = null;
            return false;
        }

        public void RegisterRelation(IDocRelation docRelation)
        {
            if (docRelation == null)
            {
                throw new ArgumentNullException("docRelation");
            }

            var entityTypes = docRelation.GetPartTypes();
            foreach (var entityType in entityTypes)
            {
                UnionMapping(entityType, docRelation.DocType);
            }
        }

        private void UnionMapping(Type entityType, Type docType)
        {
            List<Type> docTypes;
            if (!_dictionary.TryGetValue(entityType, out docTypes))
            {
                docTypes = new List<Type>();
                _dictionary.Add(entityType, docTypes);
            }

            docTypes.Add(docType);
        }
    }
}