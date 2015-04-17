using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public class DefaultDataViewFeature : IMetadataFeature
    {        
        public DefaultDataViewFeature(string dataView)
        {
            DataView = dataView;
        }

        public string DataView { get; private set; }
    }
}
