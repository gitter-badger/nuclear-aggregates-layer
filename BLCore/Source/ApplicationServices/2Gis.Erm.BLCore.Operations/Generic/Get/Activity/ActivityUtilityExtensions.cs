using DoubleGis.Erm.Platform.Model.Entities;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    internal static class ActivityUtilityExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the entity could be treated as a regarding object.
        /// </summary>
        public static bool CanBeRegardingObject(this EntityName entityName)
        {
            return entityName == EntityName.Client || entityName == EntityName.Firm || entityName == EntityName.Deal;
        }

        /// <summary>
        /// Returns a value indicating whether the entity could be treated as an attendee.
        /// </summary>
        public static bool CanBeContacted(this EntityName entityName)
        {
            return entityName == EntityName.Contact;
        }

        public static bool IsActivity(this EntityName entityName)
        {
            return entityName == EntityName.Appointment || entityName == EntityName.Letter || entityName == EntityName.Phonecall || entityName == EntityName.Task;
        }
    }
}
