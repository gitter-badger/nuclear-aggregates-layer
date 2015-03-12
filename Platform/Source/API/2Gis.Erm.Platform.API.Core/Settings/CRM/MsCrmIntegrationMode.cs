using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    [Flags]
    public enum MsCrmIntegrationMode
    {
        Disabled = 0,
        Database = 1,
        Sdk = 2,
        Full = Database | Sdk
    }
}