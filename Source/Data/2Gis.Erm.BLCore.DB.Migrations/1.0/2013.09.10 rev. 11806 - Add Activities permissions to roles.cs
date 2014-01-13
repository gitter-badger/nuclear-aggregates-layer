using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11806, "Добавляем разрешения на действия для ролей")]
    public sealed class Migration11806 : TransactedMigration
    {
        #region Identities Pool

        private static readonly long[] Identities = 
            {
                182919293841754113,
                182919293841754369,
                182919293841754625,
                182919293841754881,
                182919293841755137,
                182919293841755393,
                182919293841755649,
                182919293841755905,
                182919293841756161,
                182919293841756417,
                182919293841756673,
                182919293841756929,
                182919293841757185,
                182919293841757441,
                182919293841757697,
                182919293841757953,
                182919293841758209,
                182919293841758465,
                182919293841758721,
                182919293841758977,
                182919293841759233,
                182919293841759489,
                182919293841759745,
                182919293841760001,
                182919293841760257,
                182919293841760513,
                182919293841760769,
                182919293841761025,
                182919293841761281,
                182919293841761537,
                182919293841761793,
                182919293841762049,
                182919293841762305,
                182919293841762561,
                182919293841762817,
                182919293841763073,
                182919293841763329,
                182919293841763585,
                182919293841763841,
                182919293841764097,
                182919293841764353,
                182919293841764609,
                182919293841764865,
                182919293841765121,
                182919293841765377,
                182919293841765633,
                182919293841765889,
                182919293841766145,
                182919293841766401,
                182919293841766657,
                182919293841766913,
                182919293841767169,
                182919293841767425,
                182919293841767681,
                182919293841767937,
                182919293841768193,
                182919293841768449,
                182919293841768705,
                182919293841768961,
                182919293841769217,
                182919293841769473,
                182919293841769729,
                182919293841769985,
                182919293841770241,
                182919293841770497,
                182919293841770753,
                182919293841771009,
                182919293841771265,
                182919293841771521,
                182919293841771777,
                182919293841772033,
                182919293841772289,
                182919293841772545,
                182919293841772801,
                182919293841773057,
                182919293841773313,
                182919293841773569,
                182919293841773825,
                182919293841774081,
                182919293841774337,
                182919293841774593,
                182919293841774849,
                182919293841775105,
                182919293841775361,
                182919293841775617,
                182919293841775873,
                182919293841776129,
                182919293841776385,
                182919293841776641,
                182919293841776897,
                182919293841777153,
                182919293841777409,
                182919293841777665,
                182919293841777921,
                182919293841778177,
                182919293841778433,
                182919293841778689,
                182919293841778945,
                182919293841779201,
                182919293841779457
            };

        #endregion

        private const string SelectReadAndCreatePrivilegeIdsStatement = @"
SELECT P.Id
FROM [Security].[Privileges] P 
WHERE P.EntityType = 500 AND Operation IN (1, 32)
";

        private const string SelectUpdatePrivilegeIdStatement = @"
SELECT P.Id
FROM [Security].[Privileges] P 
WHERE P.EntityType = 500 AND Operation = 2
";

        private const string RoleIdsStatement = @"
SELECT R.Id
FROM [Security].[Roles] R
WHERE R.Id NOT IN
(
	SELECT DISTINCT R.Id
	FROM [Security].[RolePrivileges] RP
	JOIN [Security].[Privileges] P ON RP.PrivilegeId = P.Id
	JOIN [Security].[Roles] R ON RP.RoleId = R.Id
	WHERE P.EntityType = 500 
)";

        private const string SelectRolePrivilegeIdsStatementTemplate = @"
SELECT Id FROM [Security].[RolePrivileges]
WHERE Id IN ({0})
";

        private const string InsertIntoRolePrivilegesStatementTemplate = @"
INSERT INTO [Security].[RolePrivileges](Id, PrivilegeId, RoleId, Priority, Mask, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
VALUES({0}, {1}, {2}, 0, {3}, 1, GETDATE(), 1, GETDATE())
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var roleIds = new List<long>();
            var rolesReader = context.Connection.ExecuteReader(RoleIdsStatement);
            while (rolesReader.Read())
            {
                roleIds.Add(rolesReader.GetInt64(0));
            }
            
            rolesReader.Close();

            var privilegeIds = new List<long>();
            var privilegesReader = context.Connection.ExecuteReader(SelectReadAndCreatePrivilegeIdsStatement);
            while (privilegesReader.Read())
            {
                privilegeIds.Add(privilegesReader.GetInt64(0));
            }
            
            privilegesReader.Close();

            var existingRolePrivilegeIds = new List<long>();
            var stringBuilder = new StringBuilder();
            var delimitedIdentities = Identities.Aggregate(stringBuilder, (result, next) => result.Append(string.Format("{0},", next))).ToString().TrimEnd(',');
            privilegesReader = context.Connection.ExecuteReader(string.Format(SelectRolePrivilegeIdsStatementTemplate, delimitedIdentities));
            while (privilegesReader.Read())
            {
                existingRolePrivilegeIds.Add(privilegesReader.GetInt64(0));
            }

            privilegesReader.Close();

            var count = 0;
            foreach (var privilegeId in privilegeIds)
            {
                foreach (var roleId in roleIds)
                {
                    var nextIdentity = Identities[count];
                    if (!existingRolePrivilegeIds.Contains(nextIdentity))
                    {
                        // Organization
                        context.Connection.ExecuteNonQuery(string.Format(InsertIntoRolePrivilegesStatementTemplate,
                                                                         Identities[count],
                                                                         privilegeId,
                                                                         roleId,
                                                                         16));
                    }

                    ++count;
                }
            }

            privilegesReader = context.Connection.ExecuteReader(SelectUpdatePrivilegeIdStatement);
            privilegesReader.Read();
            var updatePrivilegeId = privilegesReader.GetInt64(0);
            privilegesReader.Close();

            foreach (var roleId in roleIds)
            {
                var nextIdentity = Identities[count];
                if (!existingRolePrivilegeIds.Contains(nextIdentity))
                {
                    // DepartmentAndChilds
                    context.Connection.ExecuteNonQuery(string.Format(InsertIntoRolePrivilegesStatementTemplate,
                                                                     Identities[count],
                                                                     updatePrivilegeId,
                                                                     roleId,
                                                                     8));
                }
                
                ++count;
            }
        }
    }
}
