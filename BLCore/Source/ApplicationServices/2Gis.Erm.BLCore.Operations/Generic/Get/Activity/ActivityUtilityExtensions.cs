using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    internal static class ActivityUtilityExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the entity could be treated as a regarding object.
        /// </summary>
        public static bool CanBeRegardingObject(this IEntityType entityName)
        {
            return EntityType.Instance.Equals(EntityType.Instance.Client()) || EntityType.Instance.Equals(EntityType.Instance.Firm()) || EntityType.Instance.Equals(EntityType.Instance.Deal());
        }

        /// <summary>
        /// Returns a value indicating whether the entity could be treated as an attendee.
        /// </summary>
        public static bool CanBeContacted(this IEntityType entityName)
        {
            return EntityType.Instance.Equals(EntityType.Instance.Contact());
        }
    }
}
