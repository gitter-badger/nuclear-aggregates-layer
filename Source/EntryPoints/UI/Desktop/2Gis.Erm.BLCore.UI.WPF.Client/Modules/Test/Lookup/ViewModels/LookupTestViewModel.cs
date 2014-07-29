using System;
using System.Collections.Generic;
using System.Windows.Input;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Lookup.ViewModels
{
    public sealed class LookupTestViewModel : ViewModelBase, IDocument
    {
        private long _entity1Id;
        private string _entity1Name;
        private long _entity2Id;
        private string _entity2Name;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string DocumentName
        {
            get { return "Lookup Test"; }
        }

        Guid IDocument.Id
        {
            get
            {
                return _identity.Id;
            }
        }

        public IEnumerable<INavigationItem> Actions { get; set; }

        public ICommand NavigationCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public long Entity1Id
        {
            get
            {
                return _entity1Id;
            }

            set
            {
                _entity1Id = value;
                RaisePropertyChanged();
            }
        }

        public string Entity1Name
        {
            get
            {
                return _entity1Name;
            }

            set
            {
                _entity1Name = value;
                RaisePropertyChanged();
            }
        }

        public long Entity2Id
        {
            get
            {
                return _entity2Id;
            }

            set
            {
                _entity2Id = value;
                RaisePropertyChanged();
            }
        }

        public string Entity2Name
        {
            get
            {
                return _entity2Name;
            }

            set
            {
                _entity2Name = value;
                RaisePropertyChanged();
            }
        }
    }
}
