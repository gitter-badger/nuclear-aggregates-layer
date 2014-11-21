using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public interface IUserProfilesMainViewModel
    {
        IUserLocaleInfoViewModel LocaleInfoViewModel { get; }
        IUserPersonalInfoViewModel PersonalInfoViewModel { get; }

        void Init(UserProfileViewModel viewModel);
        UserProfilesMainViewModel.SaveResult Save(long profileId);
        void Close();

    }
}
