using System;
using System.Linq;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM
{
    public static class CrmDataContextExtensions
    {
        public static IQueryable<ICrmEntity> GetEntities(this ICrmDataContext context, CrmEntityName entityName)
        {
            return context.GetEntities(entityName.GetName());
        }

        public static CrmUserInfo GetSystemUserByDomainName(this ICrmDataContext crmDataContext, string domainName, bool throwException)
        {
            var crmUserInfos = crmDataContext.GetEntities(EntityName.systemuser).Where(x => x.GetPropertyValue<string>("domainname").Contains(domainName)).Select(x => new CrmUserInfo
            {
                DomainName = x.GetPropertyValue<string>("domainname"),
                UserId = x.GetPropertyValue<Guid>("systemuserid"),
                BusinessUnitId = x.GetPropertyValue<Guid>("businessunitid"),
                InternalEmailAddress = x.GetPropertyValue<string>("internalemailaddress"),
            }).ToArray();

            var targetUser = crmUserInfos.FirstOrDefault(x => string.Equals(GetNameByDomainName(x.DomainName), domainName, StringComparison.OrdinalIgnoreCase));

            if (targetUser == null)
            {
                if (throwException)
                {
                    throw new ArgumentException(string.Format("User with domain name '{0}' cannot be found in Dynamics CRM", domainName));
                }

                return null;
            }

            return targetUser;
        }

        public static void UpdateOwner(this ICrmDataContext crmDataContext, string entityName, Guid entityid, string ownerDomainName)
        {
            var userInfo = GetSystemUserByDomainName(crmDataContext, ownerDomainName, true);

            var assignee = new SecurityPrincipal { PrincipalId = userInfo.UserId };
            var target = new TargetOwnedDynamic { EntityId = entityid, EntityName = entityName };

            crmDataContext.UsingService(service => service.Execute(new AssignRequest { Assignee = assignee, Target = target }));
        }

        public static bool TryGetCrmRoleId(this ICrmDataContext crmDataContext, Guid businessUnitid, string roleName, out Guid roleId)
        {
            var roleCode = crmDataContext.GetEntities(EntityName.role)
                .Where(x => x.GetPropertyValue<Guid>("businessunitid") == businessUnitid)
                .ToArray()
                .Where(x => x.GetPropertyValue<string>("name") == roleName)
                .Select(x => x.Id)
                .SingleOrDefault();

            if (roleCode == null)
            {
                roleId = Guid.Empty;
                return false;
            }

            roleId = roleCode.Value;
            return true;
        }

        public static ICrmEntity GetFirm(this ICrmDataContext crmDataContext, Guid firmReplicationCode)
        {
            return crmDataContext.GetEntities("dg_firm").SingleOrDefault(x => x.GetPropertyValue<Guid>("dg_firmid") == firmReplicationCode);
        }

        public static ICrmEntity GetClient(this ICrmDataContext crmDataContext, Guid clientReplicationCode)
        {
            return crmDataContext.GetEntities(EntityName.account).SingleOrDefault(x => x.GetPropertyValue<Guid>("accountid") == clientReplicationCode);
        }

        // retrieve "admin" from "crm\admin" domainname
        internal static string GetNameByDomainName(string domainName)
        {
            var backSlashIndex = domainName.IndexOf('\\');
            if (backSlashIndex == -1)
            {
                return domainName;
            }

            return domainName.Substring(backSlashIndex + 1);
        }

        public sealed class CrmUserInfo
        {
            public string DomainName { get; set; }
            public Guid UserId { get; set; }
            public string InternalEmailAddress { get; set; }
            public Guid BusinessUnitId { get; set; }
        }
    }
}
