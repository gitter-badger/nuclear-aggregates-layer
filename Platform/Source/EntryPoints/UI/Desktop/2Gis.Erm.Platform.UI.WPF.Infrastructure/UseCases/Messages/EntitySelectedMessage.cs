using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class EntitySelectedMessage : MessageBase<FreeProcessingModel>
    {
        private readonly IEntityType _entityName;
        private readonly long _entityId;

        public EntitySelectedMessage(IEntityType entityName, long entityId)
            : base(null)
        {
            _entityName = entityName;
            _entityId = entityId;
        }

        public IEntityType EntityName
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