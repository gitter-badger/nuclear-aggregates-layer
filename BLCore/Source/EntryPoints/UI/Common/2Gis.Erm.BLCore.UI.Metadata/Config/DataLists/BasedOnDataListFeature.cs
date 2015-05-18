using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class BasedOnDataListFeature : IMetadataFeature
    {
        private readonly DataListMetadata _dataListMetadata;

        public BasedOnDataListFeature(DataListMetadata dataListMetadata)
        {
            _dataListMetadata = dataListMetadata;
        }

        public DataListMetadata DataListMetadata
        {
            get { return _dataListMetadata; }
        }
    }
}