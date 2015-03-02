using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public static class AmbiguousFieldsExtenstion
    {
        public static IEnumerable<string> GetAmbiguousFields(this IEnumerable<EntityReference> references)
        {
            if (references == null)
            {
                return Enumerable.Empty<string>();
            }

            return references.Where(s => !s.Id.HasValue).Select(s => s.EntityName.AsEntityType().Name).ToList();
        }
    }
}
