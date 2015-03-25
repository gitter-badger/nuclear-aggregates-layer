using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.Elements.Identities.Builder;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation
{
    public sealed class FakeNavigationItem : INavigationItem
    {
        private readonly string _title;
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("FakeNavigationItems");

        public FakeNavigationItem(string title)
        {
            _title = title;
        }

        public Uri Id
        {
            get
            {
                return _id;
            }
        }

        public string Title 
        {
            get
            {
                return _title;
            }
        }

        public IImageProvider Icon { get; set; }
        public IDelegateCommand NavigateCommand { get; set; }
        public INavigationItem[] Items { get; set; }

        public bool IsSelected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsExpanded
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}