using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata FirmAddress =
            CardMetadata.For<FirmAddress>()
                        .MainAttribute<FirmAddress, IFirmAddressViewModel>(x => x.Address)
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("FirmContacts")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmContacts)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.FirmContact)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("CategoryFirmAddress")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAddressCategories)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.CategoryFirmAddress)
                                                             .FilterToParent());
    }
}