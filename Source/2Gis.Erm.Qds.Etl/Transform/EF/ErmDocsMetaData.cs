using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class ErmDocsMetaData : IDocsMetaData
    {
        private readonly IDocUpdatersRegistry _docUpdatersRegistry;
        private readonly ITransformRelations _transformRelations;

        public ErmDocsMetaData(IDocUpdatersRegistry docUpdatersRegistry, ITransformRelations transformRelations)
        {
            if (docUpdatersRegistry == null)
            {
                throw new ArgumentNullException("docUpdatersRegistry");
            }

            if (transformRelations == null)
            {
                throw new ArgumentNullException("transformRelations");
            }

            _docUpdatersRegistry = docUpdatersRegistry;
            _transformRelations = transformRelations;
        }

        public IDocsUpdater[] GetDocsUpdaters(Type partType)
        {
            if (partType == null)
            {
                throw new ArgumentNullException("partType");
            }

            var docTypes = GetDocTypesByEntityType(partType);

            var modifiers = new IDocsUpdater[docTypes.Length];
            for (int i = 0; i < docTypes.Length; i++)
            {
                var docType = docTypes[i];
                modifiers[i] = _docUpdatersRegistry.GetUpdater(docType);
            }

            return modifiers;
        }

        private Type[] GetDocTypesByEntityType(Type entityType)
        {
            return _transformRelations.GetRelatedDocTypes(entityType);
        }
    }
}