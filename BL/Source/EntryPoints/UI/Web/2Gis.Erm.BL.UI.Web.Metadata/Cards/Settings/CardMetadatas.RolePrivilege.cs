﻿using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata RolePrivilege =
            CardMetadata.For<RolePrivilege>()
                        .Icon.Path(Icons.Icons.Entity.RolePrivilege)
                        .CommonCardToolbar();
    }
}