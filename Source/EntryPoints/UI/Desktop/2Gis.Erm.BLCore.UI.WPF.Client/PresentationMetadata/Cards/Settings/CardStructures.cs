using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        private static readonly CardStructure[] CachedSettings;
        static CardStructures()
        {
            CachedSettings = typeof(CardStructures).Extract<CardStructure>(null).ToArray();
        }

        public static CardStructure[] Settings 
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
