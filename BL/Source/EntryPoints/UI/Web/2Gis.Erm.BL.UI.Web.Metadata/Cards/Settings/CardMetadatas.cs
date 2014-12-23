using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        private static readonly CardMetadata[] CachedSettings;
        static CardMetadatas()
        {
            CachedSettings = typeof(CardMetadatas).Extract<CardMetadata>(null).ToArray();
        }

        public static CardMetadata[] Settings
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
