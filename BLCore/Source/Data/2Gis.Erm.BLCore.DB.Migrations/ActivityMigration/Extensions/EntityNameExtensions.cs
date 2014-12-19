using System;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions
{
    using ErmEntityName = Metadata.Erm.EntityName;

    internal static class EntityNameExtensions
    {
        public static ErmEntityName ToEntityName(this string entityName)
        {
            switch (entityName)
            {
                case "account":
                    return ErmEntityName.Client;
                case "contact":
                    return ErmEntityName.Contact;
                case "opportunity":
                    return ErmEntityName.Deal;
                case "dg_firm":
                    return ErmEntityName.Firm;
                case "systemuser":
                    return ErmEntityName.User;
                case "bulkoperation":
                    return ErmEntityName.None;
                case "appointment":
                    return ErmEntityName.Appointment;
                case "phonecall":
                    return ErmEntityName.Phonecall;
                case "task":
                    return ErmEntityName.Task;
                case "letter":
                case "fax":
                case "email":
                    return ErmEntityName.Letter;
                default:
                    throw new NotSupportedException(string.Format("The entity '{0}' is not supported.", entityName));
            }
        }
         
    }
}