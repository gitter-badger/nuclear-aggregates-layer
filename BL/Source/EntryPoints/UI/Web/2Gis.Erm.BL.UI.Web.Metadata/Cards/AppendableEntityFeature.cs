using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class AppendableEntityFeature : IMetadataFeature
    {
        public AppendableEntityFeature(IEntityType entity)
        {
            Entity = entity;
        }

        public IEntityType Entity { get; private set; }
    }
}