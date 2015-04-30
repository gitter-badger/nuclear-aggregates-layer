using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class ErmConnectionStringIdentity : IdentityBase<ErmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 2; }
        }

        public override string Description
        {
            get { return "Erm storage connection string"; }
        }
    }
}