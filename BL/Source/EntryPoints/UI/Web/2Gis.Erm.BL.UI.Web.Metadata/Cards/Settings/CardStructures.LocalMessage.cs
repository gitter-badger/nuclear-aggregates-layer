using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata LocalMessage =
            CardMetadata.For<LocalMessage>()
                        .MainAttribute(x => x.Id)
                        .Actions.Attach(// COMMENT {all, 28.11.2014}: а как же безопасность?
                                        UiElementMetadata.Config
                                                         .AdditionalActions(UiElementMetadata.Config
                                                                                             .Name.Static("SaveAs")
                                                                                             .Title.Resource(() => ErmConfigLocalization.ControlSaveAs)
                                                                                             .Handler.Name("scope.SaveAs")),
                                        UiElementMetadata.Config.CloseAction());
    }
}