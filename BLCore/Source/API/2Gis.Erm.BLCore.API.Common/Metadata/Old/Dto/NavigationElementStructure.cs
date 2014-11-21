using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class NavigationElementStructure
    {
        public string NameLocaleResourceId { get; set; }
        public string LocalizedName { get; set; }

        public string Icon { get; set; }
        public string RequestUrl { get; set; }

        public IEnumerable<NavigationElementStructure> Items { get; set; }
    }
   
}