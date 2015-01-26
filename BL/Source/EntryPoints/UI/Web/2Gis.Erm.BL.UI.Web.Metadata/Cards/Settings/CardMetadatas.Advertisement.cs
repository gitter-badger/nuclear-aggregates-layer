﻿using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Advertisement =
            CardMetadata.For<Advertisement>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityName.Advertisement))
                        .InfoOn<Advertisement, ISelectableToWhiteListAspect>(x => x.IsSelectedToWhiteList,
                                                                             StringResourceDescriptor.Create(() => BLResources.AdvertisementIsSelectedToWhiteList));
    }
}