using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyMetadata Reports =
            HierarchyMetadata.Config
                .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                .Icon.Resource(Images.Navigation.NavGroupReports)
                .Childs(
                    HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                        .Icon.Resource(Images.Navigation.NavGroupReports)
                        .BindMVVM<NullViewModel,TitleOnlyContextView>());
    }
}
