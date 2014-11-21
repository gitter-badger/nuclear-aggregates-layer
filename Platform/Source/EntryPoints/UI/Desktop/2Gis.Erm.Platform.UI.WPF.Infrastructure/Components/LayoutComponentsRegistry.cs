using System;
using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Components
{
    public sealed class LayoutComponentsRegistry : ILayoutComponentsRegistry
    {
        private readonly IReadOnlyDictionary<Type, IEnumerable<ILayoutComponent>> _registry;

        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        public LayoutComponentsRegistry(ILayoutDocumentsComponent[] documentsComponents,
                                        ILayoutDocumentHeadersComponent[] documentHeadersComponents,
                                        ILayoutNavigationComponent[] navigationComponents,
                                        ILayoutToolbarComponent[] toolbarComponents,
                                        ILayoutUserInfoComponent[] userInfoComponents,
                                        ILayoutNotificationsComponent[] layoutNotificationsComponents,
                                        ILayoutDialogComponent[] layoutDialogComponents)
            // ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            _registry = GetComponentsRegistry(documentsComponents,
                                              documentHeadersComponents,
                                              navigationComponents,
                                              toolbarComponents,
                                              userInfoComponents,
                                              layoutNotificationsComponents,
                                              layoutDialogComponents);
        }

        public IEnumerable<TLayoutComponent> GetComponentsForLayoutRegion<TLayoutComponent>() 
            where TLayoutComponent : ILayoutComponent
        {
            var targetLayoutComponentType = typeof(TLayoutComponent);
            IEnumerable<ILayoutComponent> resolvedComponents;
            if (!_registry.TryGetValue(targetLayoutComponentType, out resolvedComponents))
            {
                throw new InvalidOperationException("Can't resolve layout components of type " + targetLayoutComponentType);
            }

            return (IEnumerable<TLayoutComponent>)resolvedComponents;
        }

        private IReadOnlyDictionary<Type, IEnumerable<ILayoutComponent>> GetComponentsRegistry(IEnumerable<ILayoutDocumentsComponent> documentsComponents,
                                                                                               IEnumerable<ILayoutDocumentHeadersComponent> documentHeadersComponents,
                                                                                               IEnumerable<ILayoutNavigationComponent> navigationComponents,
                                                                                               IEnumerable<ILayoutToolbarComponent> toolbarComponents,
                                                                                               IEnumerable<ILayoutUserInfoComponent> userInfoComponents,
                                                                                               IEnumerable<ILayoutNotificationsComponent> notificationsComponents,
                                                                                               IEnumerable<ILayoutDialogComponent> layoutDialogComponents)
        {
            return new Dictionary<Type, IEnumerable<ILayoutComponent>>
                {
                    { typeof(ILayoutDocumentsComponent), documentsComponents },
                    { typeof(ILayoutDocumentHeadersComponent), documentHeadersComponents },
                    { typeof(ILayoutNavigationComponent), navigationComponents },
                    { typeof(ILayoutToolbarComponent), toolbarComponents },
                    { typeof(ILayoutUserInfoComponent), userInfoComponents },
                    { typeof(ILayoutNotificationsComponent), notificationsComponents },
                    { typeof(ILayoutDialogComponent), layoutDialogComponents }
                };
        }
    }
}
