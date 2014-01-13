using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class BasedOnDataListFeature : IConfigFeature
    {
        private readonly DataListStructure _dataListStructure;

        public BasedOnDataListFeature(DataListStructure dataListStructure)
        {
            _dataListStructure = dataListStructure;
        }

        public DataListStructure DataListStructure
        {
            get { return _dataListStructure; }
        }
    }
}