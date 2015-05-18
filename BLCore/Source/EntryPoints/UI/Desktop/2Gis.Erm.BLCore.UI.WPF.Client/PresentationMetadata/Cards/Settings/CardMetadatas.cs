using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
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
