using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata LocalMessage =
            CardMetadata.For<LocalMessage>()
                        .MainAttribute(x => x.Id)
                        .Actions.Attach(UIElementMetadata.Config
                                                         .AdditionalActions(UIElementMetadata.Config
                                                                                             .Name.Static("SaveAs")
                                                                                             .Title.Resource(() => ErmConfigLocalization.ControlSaveAs)
                                                                                             .ControlType(ControlType.TextButton)
                                                                                             .Handler.Name("scope.SaveAs")),
                                        UIElementMetadata.Config.CloseAction());
    }
}