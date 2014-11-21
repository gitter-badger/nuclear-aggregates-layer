using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13634, "Настройка прав для заявки на создание или продление заказа")]
    public class Migration13634 : TransactedMigration
    {
        private const int ReadOperationId = 1;
        private const int EditOperationId = 2;
        private const int AssignOperationId = 524288;

        private const long ManagerRoleId = 5;
        private const long ServiceManagerRoleId = 13;
        private const long RgRoleId = 4;
        private const long BranchDirectorRoleId = 2;
        private const long AupRoleId = 3;
        private const long HqEmployeeId = 16;
        
        private const int UserAccessMask = 1;
        private const int DepartmentAndChildsAccessMask = 8;
        private const int OrganizationAccessMask = 16;

        private const string SetPrivilegesQueryTemplate = @"
DECLARE @PrivilegeId bigint
SET @PrivilegeId = (SELECT Id FROM [Security].[Privileges] where Operation = {0} AND EntityType = 550)

    DELETE FROM [Security].[RolePrivileges] WHERE RoleId = {1} AND PrivilegeId = @PrivilegeId

    INSERT INTO [Security].[RolePrivileges] (Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
        VALUES ({2}, {1}, @PrivilegeId, 0, {3}, 1, 1, getutcdate(), getutcdate())";

        private readonly Rule[] _rules =
            {
                new Rule
                    {
                        RoleId = ManagerRoleId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230696827073069063
                    },
                new Rule
                    {
                        RoleId = ManagerRoleId,
                        OperationCode = EditOperationId,
                        DepthMask = UserAccessMask,
                        RuleId = 230697946826408199
                    },
                new Rule
                    {
                        RoleId = ManagerRoleId,
                        OperationCode = AssignOperationId,
                        DepthMask = UserAccessMask,
                        RuleId = 230697998122746375
                    },
                new Rule
                    {
                        RoleId = ServiceManagerRoleId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698044058764039
                    },
                new Rule
                    {
                        RoleId = ServiceManagerRoleId,
                        OperationCode = EditOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698112887292935
                    },
                new Rule
                    {
                        RoleId = ServiceManagerRoleId,
                        OperationCode = AssignOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698168898028807
                    },
                new Rule
                    {
                        RoleId = RgRoleId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698217837168135
                    },
                new Rule
                    {
                        RoleId = RgRoleId,
                        OperationCode = EditOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698267564836615
                    },
                new Rule
                    {
                        RoleId = RgRoleId,
                        OperationCode = AssignOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698312846542855
                    },
                new Rule
                    {
                        RoleId = BranchDirectorRoleId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698368068749575
                    },
                new Rule
                    {
                        RoleId = BranchDirectorRoleId,
                        OperationCode = EditOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698420807928327
                    },
                new Rule
                    {
                        RoleId = BranchDirectorRoleId,
                        OperationCode = AssignOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698465426934535
                    },
                new Rule
                    {
                        RoleId = AupRoleId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698514240244743
                    },
                new Rule
                    {
                        RoleId = AupRoleId,
                        OperationCode = EditOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698563439430919
                    },
                new Rule
                    {
                        RoleId = AupRoleId,
                        OperationCode = AssignOperationId,
                        DepthMask = DepartmentAndChildsAccessMask,
                        RuleId = 230698608066825735
                    },
                new Rule
                    {
                        RoleId = HqEmployeeId,
                        OperationCode = ReadOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698653868625671
                    },
                new Rule
                    {
                        RoleId = HqEmployeeId,
                        OperationCode = EditOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698701893406727
                    },
                new Rule
                    {
                        RoleId = HqEmployeeId,
                        OperationCode = AssignOperationId,
                        DepthMask = OrganizationAccessMask,
                        RuleId = 230698747955253511
                    }
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var rule in _rules)
            {
                var query = string.Format(SetPrivilegesQueryTemplate, rule.OperationCode, rule.RoleId, rule.RuleId, rule.DepthMask);
                context.Database.ExecuteNonQuery(query);
            }
        }

        private struct Rule
        {
            public long RoleId { get; set; }
            public int OperationCode { get; set; }
            public int DepthMask { get; set; }
            public long RuleId { get; set; }
        }
    }
}
