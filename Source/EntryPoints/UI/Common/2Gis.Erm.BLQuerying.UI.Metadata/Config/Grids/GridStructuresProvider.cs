using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids
{
    public class GridStructuresProvider : IGridStructuresProvider
    {
        private readonly IReadOnlyDictionary<EntityName, GridStructure> _entity2GridMap;

        public GridStructuresProvider()
        {
           _entity2GridMap = GridStructures.Settings.ToDictionary(x => x.Identity.EntityName);
        }

        public GridStructure[] Structures
        {
            get { return _entity2GridMap.Values.ToArray(); }
        }

        public bool TryGetDescriptor(EntityName entityName, out GridStructure descriptor)
        {
            return _entity2GridMap.TryGetValue(entityName, out descriptor);
        }
    }
}