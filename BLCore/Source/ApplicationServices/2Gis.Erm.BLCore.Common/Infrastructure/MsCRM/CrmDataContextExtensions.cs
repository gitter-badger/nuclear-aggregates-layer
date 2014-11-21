using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

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

        public static string GetSystemUserNameByCode(this ICrmDataContext crmDataContext, Guid systemUserCode)
        {
            var targetUser = crmDataContext.GetEntities(EntityName.systemuser).FirstOrDefault(x => x.GetPropertyValue<Guid>("systemuserid") == systemUserCode);
            if (targetUser == null)
            {
                throw new ArgumentException(string.Format("User with code '{0}' cannot be found in Dynamics CRM", systemUserCode));
            }

            return GetNameByDomainName(targetUser.GetPropertyValue<string>("domainname"));
        }

        public static ICrmEntity GetFirm(this ICrmDataContext crmDataContext, Guid firmReplicationCode)
        {
            return crmDataContext.GetEntities("dg_firm").SingleOrDefault(x => x.GetPropertyValue<Guid>("dg_firmid") == firmReplicationCode);
        }

        public static ICrmEntity GetClient(this ICrmDataContext crmDataContext, Guid clientReplicationCode)
        {
            return crmDataContext.GetEntities(EntityName.account).SingleOrDefault(x => x.GetPropertyValue<Guid>("accountid") == clientReplicationCode);
        }

        public static CrmUserDto GetCrmUserInfo(this CrmDataContext crmDataContext, string userAccount)
        {
            var fetchXmlQuery = BuildFetchUserSettingsExpression(userAccount);

            using (var service = crmDataContext.CreateService())
            {
                var queryResult = service.Fetch(fetchXmlQuery);

                var doc = XDocument.Parse(queryResult);

                foreach (var el in doc.Root.Elements("result"))
                {
                    var accountName = IdentityUtils.GetAccount(el.Element("domainname").Value);

                    var systemUserId = new Guid(el.Element("systemuserid").Value);
                    var timezonebias = int.Parse(el.Element("systemuserid.timezonebias").Value);
                    var timezonedaylightbias = int.Parse(el.Element("systemuserid.timezonedaylightbias").Value);

                    if (accountName.IndexOf(userAccount, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        return new CrmUserDto { CrmUserId = systemUserId, TimeZoneTotalBias = TimeSpan.FromMinutes(timezonebias + timezonedaylightbias) };
                    }
                }
            }

            return null;
        }

        public static Dictionary<long, CrmUserDto> GetUserMappings(this CrmDataContext crmDataContext, Dictionary<long, string> usersDomainMapping)
        {
            var result = new Dictionary<long, CrmUserDto>(usersDomainMapping.Count);
            var userDomainNames = usersDomainMapping.Values.ToArray();

            var fetchXmlQuery = BuildFetchUserSettingsExpression(userDomainNames);

            using (var service = crmDataContext.CreateService())
            {
                var queryResult = service.Fetch(fetchXmlQuery);

                var doc = XDocument.Parse(queryResult);

                // ReSharper disable PossibleNullReferenceException
                foreach (var el in doc.Root.Elements("result"))
                {
                    var accountName = IdentityUtils.GetAccount(el.Element("domainname").Value);

                    var systemUserId = new Guid(el.Element("systemuserid").Value);
                    var timezonebias = Int32.Parse(el.Element("systemuserid.timezonebias").Value);
                    var timezonedaylightbias = Int32.Parse(el.Element("systemuserid.timezonedaylightbias").Value);

                    // ERM ные пользователи хранятся без домена (usersDomainMapping), CRM-ные (accountName) - с доменом.
                    var keyValuePair = usersDomainMapping.First(x => accountName.IndexOf(x.Value, StringComparison.InvariantCultureIgnoreCase) >= 0);

                    result[keyValuePair.Key] = new CrmUserDto { CrmUserId = systemUserId, TimeZoneTotalBias = TimeSpan.FromMinutes(timezonebias + timezonedaylightbias) };
                }
                // ReSharper restore PossibleNullReferenceException
            }

            return result;
        }

        private static string BuildFetchUserSettingsExpression(params string[] userDomainNames)
        {
            var sb = new StringBuilder(1000 + (100 * userDomainNames.Length));

            sb.Append("<fetch mapping=\"logical\" version=\"1.0\">");
            sb.Append("<entity name=\"systemuser\">");
            sb.Append("<attribute name=\"domainname\" />");
            sb.Append("<attribute name=\"systemuserid\" />");
            sb.Append("<filter type=\"or\">");

            foreach (var userDomainName in userDomainNames)
            {
                sb.AppendFormat("<condition attribute=\"domainname\" operator=\"like\" value=\"%\\{0}\" />", userDomainName);
            }

            sb.Append("</filter>");
            sb.Append("<link-entity name=\"usersettings\" from=\"systemuserid\" to=\"systemuserid\"><attribute name=\"timezonebias\" /><attribute name=\"timezonedaylightbias\" /></link-entity>");
            sb.Append("</entity>");
            sb.Append("</fetch>");
            return sb.ToString();
        }

        // retrieve "crm" from "crm\admin" domainname
        internal static string GetDomainByDomainName(string domainName)
        {
            var backSlashIndex = domainName.IndexOf('\\');
            if (backSlashIndex == -1)
            {
                return domainName;
            }

            return domainName.Substring(0, domainName.Length - backSlashIndex);
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
