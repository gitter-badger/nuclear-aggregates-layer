using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Data.Services;
using Microsoft.Xrm.Client.Services;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM
{
    public static class CrmUtils
    {
        public static CrmConnection CreateConnection(this IMsCrmSettings msCrmSettings)
        {
            return CrmConnection.Parse(msCrmSettings.CrmRuntimeConnectionString);
        }

        public static CrmDataContext CreateDataContext(this IMsCrmSettings msCrmSettings)
        {
            return new CrmDataContext(null, () => new OrganizationService(null, msCrmSettings.CreateConnection()));
        }
    }
}
