using System.Collections.Generic;

using NuClear.Model.Common.Entities;

using Newtonsoft.Json.Serialization;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class LookupSettings
    {
        public IEntityType EntityName { get; set; }
        public bool ShowReadOnlyCard { get; set; }
        public bool Disabled { get; set; }
        public bool ReadOnly { get; set; }
        public bool SupressMatchesErrors { get; set; }
        public string ExtendedInfo { get; set; }
        public IEnumerable<string> Plugins { get; set; }

        public IEntityType ParentEntityName { get; set; }
        public string ParentIdPattern { get; set; }

        public IEnumerable<LookupDataField> DataFields { get; set; }
        public string HeaderTextTemplate { get; set; }
    }
}