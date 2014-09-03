using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    // FIXME {a.rechkalov, 03.09.2014}: ����� ������� ����� ����: ChooseProfileViewModel 
    // ��������� �� ����������. �� ����� �� ������ ��� ������� � ������ (������ ��� ����� ������ ����������)
    public sealed class ChangeOrderProfilesViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField LegalPersonProfile { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }
    }
}