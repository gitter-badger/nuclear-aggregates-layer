using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;

using NuClear.Metamodeling.Elements;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public sealed class GridMetadataBuilder : MetadataElementBuilder<GridMetadataBuilder, GridMetadata>
    {
        private IEntityType _entityName;

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