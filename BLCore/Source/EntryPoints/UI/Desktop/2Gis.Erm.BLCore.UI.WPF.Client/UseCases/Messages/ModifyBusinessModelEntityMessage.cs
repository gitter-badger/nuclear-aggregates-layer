using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages
{
    public class ModifyBusinessModelEntityMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly IEntityType _entityName;
        private readonly long _entityId;

        public ModifyBusinessModelEntityMessage(IEntityType entityName, long entityId)
            :base(null)
        {
            _entityName = entityName;
            _entityId = entityId;
        }

        public IEntityType EntityName
        {
            get { return _entityName; }
        }

        public long EntityId
        {
            get { return _entityId; }
        }
    }
}