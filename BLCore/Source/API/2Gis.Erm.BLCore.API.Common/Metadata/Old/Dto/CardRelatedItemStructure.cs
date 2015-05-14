using System;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
// ReSharper restore CheckNamespace
{
    public sealed class CardRelatedItemStructure
    {
        public string Name { get; set; }
        public string NameLocaleResourceId { get; set; }
        public bool Disabled { get; set; }

        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string DisabledExpression { get; set; }
        public string LocalizedName { get; set; }
        public string Icon { get; set; }
        public string RequestUrl { get; set; }
        public string ExtendedInfo { get; set; }
        public string AppendableEntity { get; set; }
        public string DefaultDataView { get; set; }
    }
}
