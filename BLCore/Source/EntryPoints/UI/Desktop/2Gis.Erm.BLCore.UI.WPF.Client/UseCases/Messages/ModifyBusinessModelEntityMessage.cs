using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages
{
    public class ModifyBusinessModelEntityMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly EntityName _entityName;
        private readonly long _entityId;

        public ModifyBusinessModelEntityMessage(EntityName entityName, long entityId)
            :base(null)
        {
            _entityName = entityName;
            _entityId = entityId;
        }

        public EntityName EntityName
        {
            get { return _entityName; }
        }

        public long EntityId
        {
            get { return _entityId; }
        }
    }
}