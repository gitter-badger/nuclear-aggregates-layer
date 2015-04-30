using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class LoggingConnectionStringIdentity : IdentityBase<LoggingConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 1; }
        }

        public override string Description
        {
            get { return "Logging storage connection string"; }
        }
    }
}