using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata LegalPersonProfile =
            CardMetadata.For<LegalPersonProfile>()
                        .InfoOn<IMainLegalPersonProfileAspect>(x => x.IsMainProfile, StringResourceDescriptor.Create(() => BLResources.LegalPersonProfileIsMain))
                        .WithDefaultIcon()
                        .CommonCardToolbar();
    }
}