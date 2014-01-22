using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyElement Reports =
            HierarchyElement.Config
                .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                .Icon.Resource(Images.Navigation.NavGroupReports)
                .Childs(
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                        .Icon.Resource(Images.Navigation.NavGroupReports)
                        .BindMVVM<NullViewModel,TitleOnlyContextView>());
    }
}
