using System;
using System.Resources;

using DoubleGis.Platform.UI.WPF.Infrastructure.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    /// <summary>
    /// Локализатор
    /// </summary>
    public sealed class Localizer : ViewModelBase, ILocalizer, IDisposable
    {
        private readonly IUserInfo _userInfo;

        public Localizer(IUserInfo userInfo, params ResourceManager[] resourceManagers)
        {
            _userInfo = userInfo;
            Localized = new DynamicResourceDictionary(userInfo.Culture, resourceManagers);
            // когда появится необходимость реагировать на переключения культуры пользователя userInfo.Changed += OnCultureInfoChanged;
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return true;
            }
        }

        public DynamicResourceDictionary Localized { get; private set; }

        private void OnCultureInfoChanged()
        {
            Localized.Culture = _userInfo.Culture;
            RaisePropertyChanged(() => Localized);
        }

        #region Поддержка IDisposable и Finalize

        private readonly object _disposeSync = new object();
        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Free other state (managed objects).
                    // когда появится необходимость реагировать на переключения культуры пользователя userInfo.Changed -= OnCultureInfoChanged;
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }

        #endregion
    }
}