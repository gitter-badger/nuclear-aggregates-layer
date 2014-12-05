using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Metadata.Settings
{
    public interface ICardsMetadataSettings : ISettings
    {
        CardsMetadataSource CardsMetadataSource { get; }
    }
}