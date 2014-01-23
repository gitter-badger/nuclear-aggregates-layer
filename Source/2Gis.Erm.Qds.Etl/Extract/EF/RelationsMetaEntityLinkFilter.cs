using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class RelationsMetaEntityLinkFilter : IEntityLinkFilter
    {
        readonly ITransformRelations _ralations;

        public RelationsMetaEntityLinkFilter(ITransformRelations ralations)
        {
            if (ralations == null)
            {
                throw new ArgumentNullException("ralations");
            }

            _ralations = ralations;
        }

        public bool IsSupported(EntityLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException("link");
            }

            var docTypes = _ralations.GetRelatedDocTypes(link.Name.AsEntityType());
            return docTypes != null && docTypes.Length > 0;
        }
    }
}