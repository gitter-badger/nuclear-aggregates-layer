using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class RelationsMetaEntityLinkFilter : IEntityLinkFilter
    {
        readonly ITransformRelations _relations;

        public RelationsMetaEntityLinkFilter(ITransformRelations ralations)
        {
            if (ralations == null)
            {
                throw new ArgumentNullException("ralations");
            }

            _relations = ralations;
        }

        public bool IsSupported(EntityLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException("link");
            }

            var key = link.Name.AsEntityType();
            IEnumerable<Type> docTypes;
            return _relations.TryGetRelatedDocTypes(key, out docTypes);
        }
    }
}