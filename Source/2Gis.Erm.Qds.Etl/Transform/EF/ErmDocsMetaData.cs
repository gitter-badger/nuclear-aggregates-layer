using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<IDocsUpdater> GetDocsUpdaters(Type partType)
        {
            if (partType == null)
            {
                throw new ArgumentNullException("partType");
            }

            IEnumerable<Type> docTypes;
            if (!_transformRelations.TryGetRelatedDocTypes(partType, out docTypes))
            {
                throw new ArgumentNullException("partType"); // FIXME {all, 24.04.2014}: Непонятно, почему argumentnullexception? Нет теста на это исключение
            }

            var updaters = docTypes.Select(x => _docUpdatersRegistry.GetUpdater(x));
            return updaters;
        }
    }
}