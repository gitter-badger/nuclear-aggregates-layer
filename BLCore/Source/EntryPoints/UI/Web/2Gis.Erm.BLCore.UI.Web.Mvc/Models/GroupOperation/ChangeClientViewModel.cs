using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation
{
    public sealed class ChangeClientViewModel : GroupOperationViewModel
    {
        // [RequiredLocalized] 
        // NOTE: Dependency ������ ��� ���������� �������� ������������ ViewModel'��
        public LookupField Client { get; set; }
    }
}