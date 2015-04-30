using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class ErmReportsConnectionStringIdentity : IdentityBase<ErmReportsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 3; }
        }

        public override string Description
        {
            get { return "Erm Reports storage connection string"; }
        }
    }
}