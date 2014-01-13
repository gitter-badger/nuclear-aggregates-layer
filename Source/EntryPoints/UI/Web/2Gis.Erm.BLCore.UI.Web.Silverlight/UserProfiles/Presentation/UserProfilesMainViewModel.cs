using System.Windows.Browser;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.MVVM;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public class UserProfilesMainViewModel : ViewModelBase, IUserProfilesMainViewModel
    {
        [ScriptableType]
        public sealed class SaveResult
        {
            public static SaveResult Error
            {
                get
                {
                    return new SaveResult
                        {
                            ResponseText = null, 
                            IsSuccessful = false, 
                            StatusText = Resources.Resources.ErrorWhileSaving
                        };
                }
            }

            public SaveResult()
            {
                IsSuccessful = true;
            }

            [ScriptableMember]
            public string ResponseText { get; set; }
            [ScriptableMember]
            public string StatusText { get; set; }
            [ScriptableMember]
            public bool IsSuccessful { get; set; }
        }

        private readonly long _userCode;
        private readonly IUserProfileDataProvider _userProfileDataProvider;
        private UserProfileViewModel _serverSideViewModel;

        public UserProfilesMainViewModel(long userCode,
                                         IUserLocaleInfoViewModel localeInfoViewModel,
                                         IUserPersonalInfoViewModel personalInfoViewModel,
                                         IUserProfileDataProvider userProfileDataProvider)
        {
            _userCode = userCode;
            _userProfileDataProvider = userProfileDataProvider;
            LocaleInfoViewModel = localeInfoViewModel;
            PersonalInfoViewModel = personalInfoViewModel;
        }

        #region Implementation of IUserProfilesViewModel
        public IUserLocaleInfoViewModel LocaleInfoViewModel { get; private set; }
        public IUserPersonalInfoViewModel PersonalInfoViewModel { get; private set; }

        [ScriptableMember]
        public void Init(UserProfileViewModel viewModel)
        {
            _serverSideViewModel = viewModel;
        }

        [ScriptableMember]
        public SaveResult Save(long profileId)
        {
            if (profileId == 0)
            {
                string message = Resources.Resources.ProfileIdMustBeSet;
                return new SaveResult
                {
                    IsSuccessful = false,
                    ResponseText = JsonConvert.SerializeObject(new { Message = message }),
                    StatusText = message
                };
            }

            if (_serverSideViewModel.Id == 0)
            {
                _serverSideViewModel.Id = profileId;
            }

            _serverSideViewModel.Localsettings = LocaleInfoViewModel.LocalSettings;
            _serverSideViewModel.PersonalInfo = PersonalInfoViewModel.PersonalInfo;
            _serverSideViewModel.DomainAccountName = string.Empty;
            _serverSideViewModel.UserId = _userCode;

            if (LocaleInfoViewModel.LocalSettings == null)
            {
                string message = Resources.Resources.CannotSaveEmptyRegionalSettings;
                return new SaveResult
                    {
                        IsSuccessful = false,
                        ResponseText = JsonConvert.SerializeObject(new { Message = message }),
                        StatusText = message
                    };
            }

            var savedProfile = _userProfileDataProvider.SaveUserProfileInfo(_serverSideViewModel);
            if (savedProfile != null)
            {
                _serverSideViewModel = savedProfile;
            }

            var result = savedProfile != null
                             ? new SaveResult { ResponseText = JsonConvert.SerializeObject(savedProfile, new DotNetDateTimeConverter(), new StringEnumConverter()) }
                             : SaveResult.Error;
            return result;
        }

        [ScriptableMember]
        public void Close()
        {
            // do nothing
        }

        #endregion
    }
}
