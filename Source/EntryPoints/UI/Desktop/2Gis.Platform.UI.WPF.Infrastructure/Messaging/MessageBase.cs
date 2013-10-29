using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public abstract class MessageBase<TProcessingModel> : IMessage<TProcessingModel>
        where TProcessingModel : class, IMessageProcessingModel, new()
    {
        private readonly DateTime? _expirationTimeUtc;
        private readonly Guid _id = Guid.NewGuid();
        private readonly DateTime _timestampUtc = DateTime.Now;
        private readonly TProcessingModel _processingModel;

        protected MessageBase(DateTime? expirationTimeUtc)
        {
            _expirationTimeUtc = expirationTimeUtc;
            _processingModel = new TProcessingModel();
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public DateTime TimestampUtc
        {
            get
            {
                return _timestampUtc;
            }
        }

        public DateTime? ExpirationTimeUtc
        {
            get
            {
                return _expirationTimeUtc;
            }
        }

        public ProcessingModel ProcessingModel
        {
            get
            {
                return _processingModel.MessageProcessingModel;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}. Id:{1}. ProcessingModel:{2}.", GetType().Name, _id, ProcessingModel.ToString());
        }
    }
}