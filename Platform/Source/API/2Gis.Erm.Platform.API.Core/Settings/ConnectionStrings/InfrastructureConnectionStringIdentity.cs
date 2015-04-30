using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class InfrastructureConnectionStringIdentity : IdentityBase<InfrastructureConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 8; }
        }

        public override string Description
        {
            get { return "Erm Infrastructure storage connection string"; }
        }
    }
}