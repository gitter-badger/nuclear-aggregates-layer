using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class CardRelatedItemsGroupStructure
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string NameLocaleResourceId { get; set; }
        public IEnumerable<CardRelatedItemStructure> Items { get; set; }
    }
}