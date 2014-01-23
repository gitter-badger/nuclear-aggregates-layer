using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class ErmDocsMetaData : IDocsMetaData
    {
        private readonly IDocModifiersRegistry _docModifiersRegistry;
        private readonly ITransformRelations _transformRelations;

        public ErmDocsMetaData(IDocModifiersRegistry docModifiersRegistry, ITransformRelations transformRelations)
        {
            if (docModifiersRegistry == null)
            {
                throw new ArgumentNullException("docModifiersRegistry");
            }

            if (transformRelations == null)
            {
                throw new ArgumentNullException("transformRelations");
            }

            _docModifiersRegistry = docModifiersRegistry;
            _transformRelations = transformRelations;
        }

        public IDocsSelector[] GetDocsSelectors(Type partType)
        {
            if (partType == null)
            {
                throw new ArgumentNullException("partType");
            }

            var docTypes = GetDocTypesByEntityType(partType);

            var modifiers = new IDocsSelector[docTypes.Length];
            for (int i = 0; i < docTypes.Length; i++)
            {
                var docType = docTypes[i];
                modifiers[i] = _docModifiersRegistry.GetModifier(docType);
            }

            return modifiers;
        }

        private Type[] GetDocTypesByEntityType(Type entityType)
        {
            return _transformRelations.GetRelatedDocTypes(entityType);
        }
    }
}