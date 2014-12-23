using System;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntityReferenceExtensions
    {
        public static long GetId(this EntityReference reference)
        {
            if (reference == null || !reference.Id.HasValue)
            {
                throw new ArgumentException("The reference is not specified.", "reference");
            }
            return reference.Id.Value;
        }
    }
}