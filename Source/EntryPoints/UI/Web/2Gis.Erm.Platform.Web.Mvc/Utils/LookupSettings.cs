using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Web.Mvc.Utils
{
    public sealed class LookupSettings
    {
        public EntityName EntityName { get; set; }
        public String SearchFormFilterInfo { get; set; }
        public Boolean ShowReadOnlyCard;
        public Boolean Disabled { get; set; }
        public Boolean ReadOnly { get; set; }
        public String ExtendedInfo { get; set; }
        public IEnumerable<String> Plugins { get; set; }
    }
}