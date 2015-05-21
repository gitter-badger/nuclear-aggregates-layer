using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public static class LookupClientInitializationExtension
    {
        public static bool IsClientInitialization(this IEnumerable<EntityReference> references, int entityTypeId) 
        {
            if (references == null)
            {
                return false;
            }

            return references.Any(s => s != null && s.EntityTypeId == entityTypeId && !s.Id.HasValue);
        }

        public static bool IsClientInitialization(this EntityReference reference, int entityTypeId)
        {
            var references = new[] { reference };
            return IsClientInitialization(references, entityTypeId);
        }
    }
}
