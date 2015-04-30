using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class PerformedOperationsServiceBusConnectionStringIdentity : IdentityBase<PerformedOperationsServiceBusConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 7; }
        }

        public override string Description
        {
            get { return "Erm Performed Operations Service Bus storage connection string"; }
        }
    }
}