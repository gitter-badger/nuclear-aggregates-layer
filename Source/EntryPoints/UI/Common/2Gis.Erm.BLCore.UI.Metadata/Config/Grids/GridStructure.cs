using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class GridStructure : ConfigElement<GridStructure, GridStructureIdentity, GridStructuresBuilder>
    {
        private readonly EntityName _entityName;
        private readonly GridStructureIdentity _identity;
        private readonly Lazy<AttachedDataListsFeature> _attachedDataListsFeature;
        public GridStructure(EntityName entityName, IEnumerable<IConfigFeature> features)
            : base(features)
        {
            _entityName = entityName;
            _identity = new GridStructureIdentity(entityName);
            _attachedDataListsFeature = new Lazy<AttachedDataListsFeature>(() => ElementFeatures.OfType<AttachedDataListsFeature>().Single());
        }

        public override IConfigElementIdentity ElementIdentity
        {
            get { return _identity; }
        }

        public override GridStructureIdentity Identity
        {
            get { return _identity; }
        }

        public IEnumerable<DataListStructure> DataLists
        {
            get { return _attachedDataListsFeature.Value.DataLists; }
        }
    }
}