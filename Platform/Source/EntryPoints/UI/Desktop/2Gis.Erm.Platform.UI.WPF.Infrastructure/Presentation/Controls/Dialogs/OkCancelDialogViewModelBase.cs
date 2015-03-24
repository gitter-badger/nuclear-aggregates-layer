using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Dialogs
{
    public abstract class OkCancelDialogViewModelBase : ViewModelBase, IOkCancelDialogViewModel
    {
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private DelegateCommand _okCommand;
        private DelegateCommand _cancelCommand;

        protected OkCancelDialogViewModelBase(ITitleProviderFactory titleProviderFactory)
        {
            OkTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.OK));
            CancelTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.Cancel));
            CloseTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLCore.Resources.Client.Properties.Resources.Close));
        }

        public IViewModelIdentity Identity
        {
            get { return _identity; }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new DelegateCommand(OnOkCommand, CanExecuteOkCommand)); }
        }
        
        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(OnCancelCommand, CanExecuteCancelCommand)); }
        }

        public ITitleProvider OkTitle { get; private set; }
        public ITitleProvider CancelTitle { get; private set; }
        public ITitleProvider CloseTitle { get; private set; }

        public abstract ITitleProvider Title { get; }
        protected abstract bool CanExecuteOkCommand();
        protected abstract void OnOkCommand();
        protected abstract bool CanExecuteCancelCommand();
        protected abstract void OnCancelCommand();
    }
}