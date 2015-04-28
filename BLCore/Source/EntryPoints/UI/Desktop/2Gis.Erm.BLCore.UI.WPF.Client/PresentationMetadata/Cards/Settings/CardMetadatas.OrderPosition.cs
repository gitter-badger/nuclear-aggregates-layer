using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Custom.OrderPosition;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata OrderPosition =
            CardMetadata.For<OrderPosition>()
                        .MVVM.Bind<OrderPositionViewModel, OrderPositionView>()
                        .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources), typeof(ErmConfigLocalization));
    }
}
