using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation
{
    public class GroupOperationViewModel : ViewModel
    {
        public IEntityType EntityTypeName { get; set; }
        public string OperationName { get; set; }

        public string Message { get; set; }
    }
}