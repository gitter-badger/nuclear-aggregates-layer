using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class GridStructuresBuilder : ConfigElementBuilder<GridStructuresBuilder, GridStructure>
    {
        private EntityName _entityName;

        public GridStructuresBuilder For(EntityName entityName)
        {
            _entityName = entityName;
            return this;
        }

        public GridStructuresBuilder AttachDataLists(params DataListStructure[] dataLists)
        {
            var attachedDataListsFeature = Features.OfType<AttachedDataListsFeature>().SingleOrDefault();
            if (attachedDataListsFeature == null)
            {
                attachedDataListsFeature = new AttachedDataListsFeature(Enumerable.Empty<DataListStructure>());
                Features.Add(attachedDataListsFeature);
            }

            foreach (var dataList in dataLists)
            {
                attachedDataListsFeature.DataLists.Add(dataList);
            }

            return this;
        }

        protected override GridStructure Create()
        {
            return new GridStructure(_entityName, Features);
        }
    }
}