using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    // FIXME {all, 23.10.2013}: заявлен как simplified model cosumer, хотя вообще не использует сущности из simplified model - скорее это crosscuttingservice
    // уже есть типы сподобным функционалом, например, CrmDataContextExtensions, данный тип скорее должен быть таким же набором методов расширений, своего состояния у него фактически нет 
    [Obsolete]
    public class MsCrmInteractionService : IMsCrmInteractionService
    {
        private readonly CrmDataContext _crmDataContext;

        public MsCrmInteractionService(IMsCrmSettings msCrmSettings)
        {
            _crmDataContext = msCrmSettings.CreateDataContext();
        }

        public CrmUserDto GetCrmUserInfo(string userAccount)
        {
            var fetchXmlQuery = BuildFetchUserSettingsExpression(userAccount);

            using (var service = _crmDataContext.CreateService())
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

        public Dictionary<long, CrmUserDto> GetUserMappings(Dictionary<long, string> usersDomainMapping)
        {
            var result = new Dictionary<long, CrmUserDto>(usersDomainMapping.Count);
            var userDomainNames = usersDomainMapping.Values.ToArray();

            var fetchXmlQuery = BuildFetchUserSettingsExpression(userDomainNames);

            using (var service = _crmDataContext.CreateService())
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
    }
}
