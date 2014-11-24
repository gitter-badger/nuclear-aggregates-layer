using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        private static readonly CardMetadata[] CachedSettings;
        static CardStructures()
        {
            CachedSettings = typeof(CardStructures).Extract<CardMetadata>(null).ToArray();
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
