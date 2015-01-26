using System.Linq;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Settings
{
    public static partial class DocumentStructures
    {
        private static readonly DocumentMetadata[] CachedSettings;
        static DocumentStructures()
        {
            CachedSettings = typeof(DocumentStructures).Extract<DocumentMetadata>(null).ToArray();
        }

        public static DocumentMetadata[] Settings 
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
