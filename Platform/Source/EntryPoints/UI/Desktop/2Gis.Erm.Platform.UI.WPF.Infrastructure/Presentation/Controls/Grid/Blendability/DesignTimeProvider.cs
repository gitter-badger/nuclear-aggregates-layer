using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Blendability
{
    public static class DesignTimeProvider
    {
        public static IPager Pager 
        {
            get
            {
                var pager = new PagerViewModel(new TitleProviderFactory(NullUserInfo.Default));
                pager.UpdatePager(5, 10);
                return pager;
            }
        }

        public static IFilter Filter
        {
            get { return new FilterViewModel(new TitleProviderFactory(NullUserInfo.Default));}
        }

        public static IListSelector ViewSelector
        {
            get
            {
                var views = new List<DataViewViewModel>();
                for (int i = 1; i <= 5; i++)
                {
                    var newSource = new DataListStructure {Name = "list_", LocalizedName = "list_" + i};
                    views.Add(DataViewViewModel.FromDataViewJson(newSource));
                }

                var vm = new ListSelectorViewModel(views.ToArray(), new TitleProviderFactory(NullUserInfo.Default)) { SelectedView = views.First() };
                return vm;
            }
        }
    }
}
