using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

using NuClear.Metamodeling.Domain.Elements.Identities.Builder;
using NuClear.Metamodeling.Elements.Aspects.Conditions;
using NuClear.Metamodeling.Elements.Concrete.References;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

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
                            .Handler.ShowGrid(EntityType.Instance.OrderPosition(), "OrderId={Id}", null));
    }
}
