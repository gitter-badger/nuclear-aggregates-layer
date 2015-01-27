using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class LookupSettings
    {
        public EntityName EntityName { get; set; }
        public bool ShowReadOnlyCard { get; set; }
        public bool Disabled { get; set; }
        public bool ReadOnly { get; set; }
        public bool SupressMatchesErrors { get; set; }
        public string ExtendedInfo { get; set; }
        public IEnumerable<string> Plugins { get; set; }

        public EntityName ParentEntityName { get; set; }
        public string ParentIdPattern { get; set; }
        public IEnumerable<LookupSortInfo> DefaultSortFields { get; set;} 
    }
}