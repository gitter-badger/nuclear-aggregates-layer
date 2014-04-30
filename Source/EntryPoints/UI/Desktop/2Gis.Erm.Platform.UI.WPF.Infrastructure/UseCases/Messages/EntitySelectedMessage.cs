using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class EntitySelectedMessage : MessageBase<FreeProcessingModel>
    {
        private readonly EntityName _entityName;
        private readonly long _entityId;

        public EntitySelectedMessage(EntityName entityName, long entityId)
            : base(null)
        {
            _entityName = entityName;
            _entityId = entityId;
        }

        public EntityName EntityName
        {
            get
            {
                return _entityName;
            }
        }

        public long EntityId
        {
            get
            {
                return _entityId;
            }
        }
    }
}