using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Util;
using DoubleGis.Platform.UI.WPF.Shell.DI;
using DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell;

namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Blendability
{
    public static class DesignTimeViewModelLocator
    {
        private static readonly Lazy<IShellViewModel> ResolvedShellViewModel = new Lazy<IShellViewModel>(ResolveViewModel);
        
        static DesignTimeViewModelLocator()
        {
            ValidateExecutionMode();
        }

        /// <summary>
        /// Get design time shell view model
        /// </summary>
        public static IShellViewModel ViewModel
        {
            get
            {
                ValidateExecutionMode();

                return ResolvedShellViewModel.Value;
            }
        }
        
        private static IShellViewModel ResolveViewModel()
        {
            return Bootstrapper.DesignTimeResolveShellViewModel();
        }

        private static void ValidateExecutionMode()
        {
            if (!DesignTimeUtils.IsDesignMode)
            {
                throw new InvalidOperationException("Only design time mode access to locator supported");
            }
        }
    }
}
