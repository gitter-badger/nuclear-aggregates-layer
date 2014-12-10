using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(24482, "Removes privileges to activities except the reading.", "s.pomadin")]
    public class ActivityPrivilegesRemoving : IContextedMigration<ICrmMigrationContext>
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmConnection; }
        }

        public void Revert(ICrmMigrationContext context)
        {
            throw new NotImplementedException();
        }

        public void Apply(ICrmMigrationContext context)
        {
            using (var service = context.CrmContext.CreateService())
            {
                foreach (var roleId in RequestRoleIds(service).ToList())
                {
                    // admin and supporter roles cannot be updated
                    if (roleId == SystemRole.Admin || roleId == SystemRole.Supporter) continue;

                    foreach (var privilegeId in new[]
                        {
                            ActivityPrivilege.Create, 
                            ActivityPrivilege.Delete, 
                            ActivityPrivilege.Write, 
                            ActivityPrivilege.Append, 
                            ActivityPrivilege.AppendTo, 
                            ActivityPrivilege.Assign, 
                            ActivityPrivilege.Share, 
                        })
                    {
                        try
                        {
                            service.Execute(new RemovePrivilegeRoleRequest { RoleId = roleId, PrivilegeId = privilegeId });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("The removing of '{1}' privilege was failed for rhe role '{0}'.", roleId, privilegeId);
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Guid> RequestRoleIds(ICrmService service)
        {
            var query = new QueryExpression
            {
                EntityName = "role",
                Criteria = new FilterExpression { Conditions = { new ConditionExpression("parentroleid", ConditionOperator.Null, null) } },
            };

            return service.LoadEntities(query).Select(x => x.Value("roleid")).Where(x => x != null).Cast<Key>().Select(x => x.Value);
        }

        private static class SystemRole
        {
            public static readonly Guid Admin = new Guid("94744C2A-FCB8-E011-8548-00155D163D01");
            public static readonly Guid Supporter = new Guid("3B7B4C2A-FCB8-E011-8548-00155D163D01");
        }

        private static class ActivityPrivilege
        {
            public static readonly Guid Create = new Guid("091DF793-FE5E-44D4-B4CA-7E3F580C4664");
            public static readonly Guid Delete = new Guid("BB4457F2-9B45-4482-A95A-7ADEF25F388A");
            public static readonly Guid Read = new Guid("650C14FE-3521-45FE-A000-84138688E45D");
            public static readonly Guid Write = new Guid("0DC8F72C-57D5-4B4D-8892-FE6AAC0E4B81");
            public static readonly Guid Append = new Guid("78777C10-09AB-4326-B4C8-CF5729702937");
            public static readonly Guid AppendTo = new Guid("6EC8E901-D770-44C0-8F12-D07425F638BD");
            public static readonly Guid Assign = new Guid("8B99344E-EBBF-4F84-8438-E1E34D194DE9");
            public static readonly Guid Share = new Guid("B5F2EE06-D359-4495-BBDA-312AAE1C6B1E");
        }
    }
}
