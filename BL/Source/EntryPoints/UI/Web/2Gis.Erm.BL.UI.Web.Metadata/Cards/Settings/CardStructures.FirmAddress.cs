using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata FirmAddress =
            CardMetadata.For<FirmAddress>()
                        .MainAttribute<FirmAddress, IFirmAddressViewModel>(x => x.Address)
                        .ConfigCommonCardToolbar()
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("FirmContacts")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmContacts)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.FirmContact)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("CategoryFirmAddress")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAddressCategories)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.CategoryFirmAddress)
                                                             .FilterToParent());
    }
}