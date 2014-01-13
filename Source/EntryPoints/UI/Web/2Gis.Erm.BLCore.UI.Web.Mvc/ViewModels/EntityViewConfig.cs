
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;

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
        public CardJson CardSettings { get; set; }
    }
}

