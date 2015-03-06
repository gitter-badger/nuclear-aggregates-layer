using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public static class LookupAutoUpdateExtension
    {
        public static bool IsAutoUpdate(this IEnumerable<EntityReference> references, EntityName entityName) 
        {
            if (references == null)
            {
                return false;
            }

            return references.Any(s => s.EntityName == entityName && !s.Id.HasValue);
        }

        public static bool IsAutoUpdate(this EntityReference reference, EntityName entityName)
        {
            var references = new[] { reference };
            return IsAutoUpdate(references, entityName);
        }
    }
}
