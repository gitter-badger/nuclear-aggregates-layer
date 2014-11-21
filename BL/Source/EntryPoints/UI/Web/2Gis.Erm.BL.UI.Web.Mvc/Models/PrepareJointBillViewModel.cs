using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PrepareJointBillViewModel : ViewModel
    {
        public long EntityId { get; set; }
        public EntityName EntityName { get; set; }
        public bool IsMassBillCreateAvailable { get; set; }
        public long? ProfileId { get; set; }
    }
}