using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Lock =
            CardMetadata.For<Lock>()
                        .ErrorOn<INewableAspect>(x => x.IsNew, StringResourceDescriptor.Create(() => BLResources.CreateOrEditLockFromUINotSupported))
                        .WithDefaultIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.LockDetail, () => ErmConfigLocalization.CrdRelLockDetails));
    }
}