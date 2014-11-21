using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation
{
    public sealed class ChangeClientViewModel : GroupOperationViewModel
    {
        // [RequiredLocalized] 
        // NOTE: Dependency убраны для уменьшения иерархии наследования ViewModel'ей
        public LookupField Client { get; set; }
    }
}