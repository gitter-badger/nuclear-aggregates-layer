using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class MsCrmConnectionStringIdentity : IdentityBase<MsCrmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 4; }
        }

        public override string Description
        {
            get { return "MS Dynamics CRM storage connection string"; }
        }
    }
}