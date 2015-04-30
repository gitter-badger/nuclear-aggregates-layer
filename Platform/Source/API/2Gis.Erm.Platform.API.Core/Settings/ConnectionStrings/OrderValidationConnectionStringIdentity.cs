using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class OrderValidationConnectionStringIdentity : IdentityBase<OrderValidationConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 5; }
        }

        public override string Description
        {
            get { return "Erm Order Validation storage connection string"; }
        }
    }
}