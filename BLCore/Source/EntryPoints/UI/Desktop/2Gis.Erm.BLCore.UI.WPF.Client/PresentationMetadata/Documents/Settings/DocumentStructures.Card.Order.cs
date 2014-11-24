using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.References;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Settings
{
    public static partial class DocumentStructures
    {
        public static DocumentMetadata CreateOrUpdateOrder =
            DocumentMetadata.Config
                .Title.Static("TEST_Modify_Order")
                .MVVM.Bind<CompositeDocumentViewModel, CompositeDocumentView>()
                .Operation.SpecificFor<ModifyBusinessModelEntityIdentity, Order>()
                .Childs(
                    MetadataReference.Config.For(MetadataCardsIdentity.Instance.IdFor<Order>()),
                    AttachedMetadata.Config
                            .Title.Resource(() => ErmConfigLocalization.EnMOrderPositions)
                            .ApplyCondition(new StringConditionCondition("{Id} != 0"))
                            .Handler.ShowGrid(OrderAggregate.OrderPosition.AsEntityName()));
    }
}
