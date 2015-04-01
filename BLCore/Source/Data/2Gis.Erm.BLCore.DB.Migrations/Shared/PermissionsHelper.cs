using System;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class PermissionsHelper
    {
        public static StringBuilder InsertPrivilege(StringBuilder sb, Int32 privelegeId, String privelegeName)
        {
            sb.AppendFormat("SET IDENTITY_INSERT [Security].[Privileges] ON " +
                            "INSERT INTO [Security].[Privileges]( " +
                            "[Id], [PrivilegeType], [FunctionalLocalResourceName], [IsDeleted], [IsActive], [CreatedBy], [ModifiedBy], [CreatedOn], [ModifiedOn]) " +
                            "VALUES( " +
                            "{0}, 'F', '{1}', 0, 1, -1, NULL, GETDATE(), NULL) " +
                            "SET IDENTITY_INSERT [Security].[Privileges] OFF ",
                            privelegeId, privelegeName);
            return sb;
        }

        public static StringBuilder InsertPrivilegeDepth(StringBuilder sb, Int32 privelegeId, String privelegeGrantedName, Int32 privelegeMaskId)
        {
            sb.AppendFormat("INSERT INTO [Security].[FunctionalPrivilegeDepths] ( " +
                            "[PrivilegeId], [LocalResourceName], [Priority], [Mask], [IsDeleted], [IsActive], [CreatedBy], [ModifiedBy], [CreatedOn], [ModifiedOn]) " +
                            "VALUES ({0}, '{1}', 1, {2}, 0, 1, -1, NULL, GETDATE(), NULL) ",
                            privelegeId, privelegeGrantedName, privelegeMaskId);
            return sb;
        }

        public static bool CheckPermissionExistence(IMigrationContext context, Int32 privelegeId)
        {
            var queryText = string.Format("SELECT 1 FROM [Security].[Privileges] WHERE Id = {0}", privelegeId);
            var permission = context.Connection.ExecuteScalar(queryText);
            return permission == null;
        }
    }
}
