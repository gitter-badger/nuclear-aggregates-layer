using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static OldUIElementMetadata Reports =
            OldUIElementMetadata.Config
                .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                .Icon.Resource(Images.Navigation.NavGroupReports)
                .Childs(
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupReports)
                        .Icon.Resource(Images.Navigation.NavGroupReports)
                        .BindMVVM<NullViewModel,TitleOnlyContextView>());
    }
}
