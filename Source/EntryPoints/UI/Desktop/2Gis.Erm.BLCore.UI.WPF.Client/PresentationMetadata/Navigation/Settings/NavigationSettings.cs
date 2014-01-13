using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        private readonly static HierarchyElement[] CachedSettings;

        static NavigationSettings()
        {
            CachedSettings = new[]{ Billing, Reports, Administration };
                //автоматически не заполняем, т.к. нарушается сортировка - элементов немного пока заполняем вручную 
                //typeof(NavigationSettings).Extract<HierarchyElement>(null).ToArray();
        }

        public static HierarchyElement[] Settings
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
