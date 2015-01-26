using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyMetadata Administration =
            HierarchyMetadata.Config
                .Title.Resource(() => ErmConfigLocalization.NavAreaSettings)
                .Icon.Resource(Images.Navigation.NavAreaSettings)
                .Childs(
                    HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupServices)
                        .Icon.Resource(Images.Navigation.NavGroupServices)
                        .Childs(
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupErmAdministration)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupSecurityAdministration)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupCatalogueManagement)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupMessageQueueManagement)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>()),  
                     HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupSecurity)
                        .Childs(
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupUsers)
                                .Icon.Resource(Images.Navigation.NavGroupUsers)
                                .Handler.ShowGrid(UserAggregate.Root, null, null),
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupRoles)
                                .Icon.Resource(Images.Navigation.NavGroupRoles)
                                .Handler.ShowGrid(RoleAggregate.Root, null, null),
                            HierarchyMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupDepartments)
                                .Icon.Resource(Images.Navigation.NavGroupDepartments)
                                .Handler.ShowGrid(EntityType.Instance.Department(), null, null)));
        
    }
}