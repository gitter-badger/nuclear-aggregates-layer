using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists.WebClientSpecific
{
    public class AttachedScriptsFeature : IWebClientMetadataFeature
    {
        private readonly List<string> _scriptFileNames;

        public AttachedScriptsFeature(params string[] scriptFileNames)
        {
            _scriptFileNames = new List<string>(scriptFileNames);
        }

        public ICollection<string> ScriptFileNames
        {
            get { return _scriptFileNames; }
        }
    }
}