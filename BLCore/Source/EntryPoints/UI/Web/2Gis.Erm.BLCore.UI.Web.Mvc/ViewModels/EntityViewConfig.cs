
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public sealed partial class EntityViewConfig
    {
        public long? Id { get; set; }
        public bool ReadOnly { get; set; }
        public long? PId { get; set; }
        public string ExtendedInfo { get; set; }
        public CardStructure CardSettings { get; set; }
    }
}

