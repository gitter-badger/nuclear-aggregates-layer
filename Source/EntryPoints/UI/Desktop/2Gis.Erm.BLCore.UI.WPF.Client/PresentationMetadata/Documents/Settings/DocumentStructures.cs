using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Settings
{
    public static partial class DocumentStructures
    {
        private static readonly DocumentStructure[] CachedSettings;
        static DocumentStructures()
        {
            CachedSettings = typeof(DocumentStructures).Extract<DocumentStructure>(null).ToArray();
        }

        public static DocumentStructure[] Settings 
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}
