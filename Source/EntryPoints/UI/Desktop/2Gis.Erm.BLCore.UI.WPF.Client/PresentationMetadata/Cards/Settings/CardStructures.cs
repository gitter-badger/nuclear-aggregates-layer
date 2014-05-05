﻿using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
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
