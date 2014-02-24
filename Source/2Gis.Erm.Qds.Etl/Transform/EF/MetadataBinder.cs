using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class MetadataBinder : IMetadataBinder
    {
        private readonly IDocUpdatersRegistry _updaters;
        private readonly ITransformRelations _relations;

        public MetadataBinder(IDocUpdatersRegistry updaters, ITransformRelations relations)
        {
            if (updaters == null)
            {
                throw new ArgumentNullException("updaters");
            }
            if (relations == null)
            {
                throw new ArgumentNullException("relations");
            }

            _updaters = updaters;
            _relations = relations;
        }

        public void BindMetadata(IEnumerable<IQdsComponent> qdsComponents)
        {
            if (qdsComponents == null)
            {
                throw new ArgumentNullException("qdsComponents");
            }

            foreach (var qds in qdsComponents)
            {
                RegisterQdsComponent(_updaters, _relations, qds);
            }
        }

        private static void RegisterQdsComponent(IDocUpdatersRegistry docUpdatersRegistry,
                                                     ITransformRelations transformRelations,
                                                     IQdsComponent qdsComponent)
        {
            docUpdatersRegistry.AddUpdater(qdsComponent.CreateDocUpdater());
            transformRelations.RegisterRelation(qdsComponent.PartsDocRelation);
        }
    }
}