using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation
{
    public class GroupOperationViewModel : ViewModel
    {
        public EntityName EntityTypeName { get; set; }
        public string OperationName { get; set; }

        public string Message { get; set; }
    }
}