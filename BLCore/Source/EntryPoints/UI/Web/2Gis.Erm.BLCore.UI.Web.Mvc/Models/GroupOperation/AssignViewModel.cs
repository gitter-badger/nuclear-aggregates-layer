namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation
{
    public sealed class AssignViewModel : OwnerGroupOperationViewModel
    {
        public bool PartialAssignSupported { get; set; }
        public bool IsCascadeAssignForbidden { get; set; }
        public bool IsPartialAssign { get; set; }
    }
}