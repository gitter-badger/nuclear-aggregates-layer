using System;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
// ReSharper restore CheckNamespace
{
    public sealed class CardStructure
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public string EntityName { get; set; }
        public string EntityLocalizedName { get; set; }

        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string EntityMainAttribute { get; set; }

        public bool HasComments { get; set; }
        public bool HasAdminTab { get; set; }

        // COMMENT {all, 20.11.2014}: Есть мнение, что этому тут не место
        public int DecimalDigits { get; set; }

        public ToolbarElementStructure[] CardToolbar { get; set; }
        public CardRelatedItemsGroupStructure[] CardRelatedItems { get; set; }

        // [JsonIgnore] fields
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string TitleResourceId { get; set; }
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string EntityNameLocaleResourceId { get; set; }
    }
}