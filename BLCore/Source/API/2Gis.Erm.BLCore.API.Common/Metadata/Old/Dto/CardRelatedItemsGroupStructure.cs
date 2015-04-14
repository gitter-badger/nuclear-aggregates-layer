using System;
using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
// ReSharper restore CheckNamespace
{
    public sealed class CardRelatedItemsGroupStructure
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }

        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string NameLocaleResourceId { get; set; }
        public IEnumerable<CardRelatedItemStructure> Items { get; set; }
    }
}