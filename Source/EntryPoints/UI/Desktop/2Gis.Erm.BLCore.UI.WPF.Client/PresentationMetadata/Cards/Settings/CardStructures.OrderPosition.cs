using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Custom.OrderPosition;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardStructure OrderPosition =
            CardStructure.Config
                .For(OrderAggregate.OrderPosition.AsEntityName())
                .Title.Resource(() => ErmConfigLocalization.EnOrderPositions)
                .MVVM.Bind<OrderPositionViewModel, OrderPositionView>()
                .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources), typeof(ErmConfigLocalization));
    }
}
