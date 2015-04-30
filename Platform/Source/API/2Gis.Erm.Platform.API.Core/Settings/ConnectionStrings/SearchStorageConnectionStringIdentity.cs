using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class SearchStorageConnectionStringIdentity : IdentityBase<SearchStorageConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 6; }
        }

        public override string Description
        {
            get { return "Erm Search storage connection string"; }
        }
    }
}