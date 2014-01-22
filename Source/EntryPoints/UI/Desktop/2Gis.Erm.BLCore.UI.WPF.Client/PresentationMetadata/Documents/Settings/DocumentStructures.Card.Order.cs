using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Settings
{
    public static partial class DocumentStructures
    {
        public static DocumentStructure CreateOrUpdateOrder =
            DocumentStructure.Config
                .Title.Static("TEST_Modify_Order")
                .MVVM.Bind<CompositeDocumentViewModel, CompositeDocumentView>()
                .Operation.EntitySpecific<ModifyBusinessModelEntityIdentity>(OrderAggregate.Order.AsEntityName())
                .Compose(ReferencedElementStructure.Config
                            .For(new CardStructureIdentity(OrderAggregate.Order.AsEntityName())))
                .Compose(AttachedElementStructure.Config
                            .Title.Resource(() => ErmConfigLocalization.EnMOrderPositions)
                            .ApplyCondition(new StringConditionCondition("{Id} != 0"))
                            .Handler.ShowGrid(OrderAggregate.OrderPosition.AsEntityName(), "OrderId={Id}", null));
    }
}
