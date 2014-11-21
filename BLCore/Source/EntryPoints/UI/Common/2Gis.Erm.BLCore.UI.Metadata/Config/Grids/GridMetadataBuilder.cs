using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public sealed class GridMetadataBuilder : MetadataElementBuilder<GridMetadataBuilder, GridMetadata>
    {
        private EntityName _entityName;

        public GridMetadataBuilder For<TEntity>()
            where TEntity : class, IEntity
        {
            _entityName = typeof(TEntity).AsEntityName();
            return this;
        }

        public GridMetadataBuilder AttachDataLists(params DataListMetadata[] dataLists)
        {
            var attachedDataListsFeature = Features.OfType<AttachedDataListsFeature>().SingleOrDefault();
            if (attachedDataListsFeature == null)
            {
                attachedDataListsFeature = new AttachedDataListsFeature(Enumerable.Empty<DataListMetadata>());
                WithFeatures(attachedDataListsFeature);
            }

            foreach (var dataList in dataLists)
            {
                attachedDataListsFeature.DataLists.Add(dataList);
            }

            return this;
        }

        protected override GridMetadata Create()
        {
            return new GridMetadata(_entityName, Features);
        }
    }
}