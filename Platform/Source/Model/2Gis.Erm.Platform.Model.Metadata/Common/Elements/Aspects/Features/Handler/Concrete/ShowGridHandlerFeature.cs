using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete
{
    public sealed class ShowGridHandlerFeature : IHandlerFeature
    {
        private readonly EntityName _entityName;

        public ShowGridHandlerFeature(EntityName entityName)
        {
            _entityName = entityName;
        }

        public EntityName EntityName
        {
            get
            {
                return _entityName;
            }
        }
    }
}
