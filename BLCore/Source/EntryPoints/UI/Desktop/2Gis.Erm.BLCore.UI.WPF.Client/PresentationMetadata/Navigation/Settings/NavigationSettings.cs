
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        private static readonly HierarchyMetadata CachedSettings;

        static NavigationSettings()
        {
            CachedSettings = 
                HierarchyMetadata.Config
                    .Id.Is(IdBuilder.For<MetadataNavigationIdentity>())
                    .Childs(Billing, Reports, Administration);

                //автоматически не заполняем, т.к. нарушается сортировка - элементов немного пока заполняем вручную 
                //typeof(NavigationSettings).Extract<HierarchyElement>(null).ToArray();
        }

        public static HierarchyMetadata Settings
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
