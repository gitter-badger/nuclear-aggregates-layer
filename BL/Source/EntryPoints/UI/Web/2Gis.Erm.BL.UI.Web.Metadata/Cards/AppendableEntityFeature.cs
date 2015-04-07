using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class AppendableEntityFeature : IMetadataFeature
    {
        public AppendableEntityFeature(EntityName entity)
        {
            Entity = entity;
        }

        public EntityName Entity { get; private set; }
    }
}