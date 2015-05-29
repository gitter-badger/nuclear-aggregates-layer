using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PrepareJointBillViewModel : ViewModel
    {
        public long EntityId { get; set; }
        public IEntityType EntityName { get; set; }
        public bool IsMassBillCreateAvailable { get; set; }
        public long? ProfileId { get; set; }
    }
}