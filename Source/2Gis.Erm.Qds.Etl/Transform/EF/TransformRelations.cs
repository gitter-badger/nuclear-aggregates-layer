using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class TransformRelations : ITransformRelations
    {
        private readonly IDictionary<Type, Type[]> _dictionary = new Dictionary<Type, Type[]>();

        public Type[] GetRelatedDocTypes(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            Type[] docTypes;
            return !_dictionary.TryGetValue(entityType, out docTypes) ? new Type[0] : docTypes;
        }

        public void RegisterRelation(IDocRelation docRelation)
        {
            if (docRelation == null)
            {
                throw new ArgumentNullException("docRelation");
            }

            var entityTypes = docRelation.GetPartTypes();
            var docType = docRelation.GetDocType();

            foreach (var entityType in entityTypes)
            {
                UnionMapping(entityType, docType);
            }
        }

        private void UnionMapping(Type entityType, Type docType)
        {
            var newMapping = new[] { docType };
            if (_dictionary.ContainsKey(entityType))
            {
                var oldMapping = _dictionary[entityType];
                newMapping = new Type[oldMapping.Length + 1];

                oldMapping.CopyTo(newMapping, 0);

                newMapping[newMapping.Length - 1] = docType;
            }

            _dictionary[entityType] = newMapping;
        }
    }
}