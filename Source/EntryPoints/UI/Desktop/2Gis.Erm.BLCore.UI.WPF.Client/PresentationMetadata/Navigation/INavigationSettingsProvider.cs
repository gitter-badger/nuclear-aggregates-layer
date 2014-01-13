using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public interface INavigationSettingsProvider
    {
        HierarchyElement[] Settings { get; }
    }
}
