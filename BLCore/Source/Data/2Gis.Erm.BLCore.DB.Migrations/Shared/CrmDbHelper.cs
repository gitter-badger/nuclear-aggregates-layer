using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class CrmDbHelper
    {
        public static InsertDataExpression GenerateNewCustomView(
            string name, 
            string fetchxml, 
            string layoutXml, 
            Guid userId, 
            Guid organizationId, 
            int returnedTypeCode)
        {
            return new InsertDataExpression(CrmTableNames.SavedQueryBase)
                {
                    Rows =
                        {
                            new InsertionDataDefinition()
                                .Add("SavedQueryId", Guid.NewGuid())
                                .Add("Name", name)
                                .Add("OrganizationId", organizationId)
                                .Add("Description", null)
                                .Add("QueryType", 0)
                                .Add("IsDefault", false)
                                .Add("ReturnedTypeCode", returnedTypeCode)
                                .Add("IsUserDefined", true)
                                .Add("FetchXml", fetchxml)
                                .Add("IsCustomizable", false)
                                .Add("IsQuickFindQuery", false)
                                .Add("ColumnSetXml", null)
                                .Add("LayoutXml", layoutXml)
                                .Add("QueryAPI", string.Empty)
                                .Add("CreatedBy", userId)
                                .Add("CreatedOn", DateTime.Now)
                                .Add("ModifiedBy", userId)
                                .Add("ModifiedOn", DateTime.Now)
                                .Add("IsPrivate", false)
                                .Add("CustomizationLevel", 1)
                                .Add("SavedQueryIdUnique", Guid.NewGuid())
                                .Add("InProduction", true)
                        }
                };
        }

        public static FieldsForCustomView GetFieldsForCustomView(IMigrationContext context)
        {
            Guid crmAdminId;
            Guid organizationId;
            Guid reserveUserId;
            using (var reader = context.Connection.ExecuteReader(
                "SELECT TOP 1 SystemUserId, OrganizationId FROM dbo.SystemUserBase WHERE DomainName LIKE '%\\CRMAdministrator'"))
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Cannot find CRM systemuser '2GIS\\CRMAdministrator'");
                }

                crmAdminId = (Guid)reader["SystemUserId"];
                organizationId = (Guid)reader["OrganizationId"];
            }

            using (var reader = context.Connection.ExecuteReader(
                "SELECT TOP 1 SystemUserId FROM dbo.SystemUserBase WHERE DomainName LIKE '%\\reserve'"))
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Cannot find CRM systemuser 'reserve'");
                }

                reserveUserId = (Guid)reader["SystemUserId"];
            }

            return new FieldsForCustomView(crmAdminId, organizationId, reserveUserId);
        }

        public class FieldsForCustomView
        {
            public FieldsForCustomView(
                Guid crmAdministratorId,
                Guid crmAdministratorOrganizationId,
                Guid reserveUserId)
            {
                CrmAdministratorId = crmAdministratorId;
                CrmAdministratorOrganizationId = crmAdministratorOrganizationId;
                ReserveUserId = reserveUserId;
            }

            public Guid CrmAdministratorId { get; private set; }
            public Guid CrmAdministratorOrganizationId { get; private set; }
            public Guid ReserveUserId { get; private set; }
        }
    }
}
