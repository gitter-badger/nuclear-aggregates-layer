using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyElement Administration =
            HierarchyElement.Config
                .Title.Resource(() => ErmConfigLocalization.NavAreaSettings)
                .Icon.Resource(Images.Navigation.NavAreaSettings)
                .Childs(
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupServices)
                        .Icon.Resource(Images.Navigation.NavGroupServices)
                        .Childs(
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupErmAdministration)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupSecurityAdministration)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupCatalogueManagement)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>(),
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupMessageQueueManagement)
                                .BindMVVM<NullViewModel,TitleOnlyContextView>()),  
                     HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupSecurity)
                        .Childs(
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupUsers)
                                .Icon.Resource(Images.Navigation.NavGroupUsers)
                                .Handler.ShowGrid(UserAggregate.User.AsEntityName(), null, null),
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupRoles)
                                .Icon.Resource(Images.Navigation.NavGroupRoles)
                                .Handler.ShowGrid(RoleAggregate.Role.AsEntityName(), null, null),
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.NavGroupDepartments)
                                .Icon.Resource(Images.Navigation.NavGroupDepartments)
                                .Handler.ShowGrid(UserAggregate.Department.AsEntityName(), null, null)));
        
    }
}