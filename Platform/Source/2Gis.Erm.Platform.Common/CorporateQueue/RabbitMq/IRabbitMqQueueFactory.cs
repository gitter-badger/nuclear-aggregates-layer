using System;
using System.Collections.Generic;
using System.IO;

namespace DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq
{
    public interface IRabbitMqQueueFactory
    {
        IRabbitMqQueueReader CreateQueueReader(string name);
        IRabbitMqQueueWriter CreateQueueWriter(string name);
    }

    public interface IRabbitMqQueueReader : IEnumerable<RabbitMqCorporateMessage>, IDisposable
    {
        string Name { get; }
        IEnumerable<RabbitMqCorporateMessage> ReadAndRemove();
    }

    public interface IRabbitMqQueueWriter : IDisposable
    {
        string Name { get; }
        void Write(byte[] message);
        void Write(string key, byte[] message);
    }

    public struct RabbitMqCorporateMessage
    {
        private readonly byte[] _bytes;
        public byte[] Bytes { get { return _bytes; } }

        internal ulong DeliveryTag { get; private set; }

        public Stream CreateStream()
        {
            return new MemoryStream(_bytes);
        }

        internal RabbitMqCorporateMessage(ulong deliveryTag, byte[] bytes)
            : this()
        {
            DeliveryTag = deliveryTag;
            _bytes = bytes;
        }

        #region equality

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RabbitMqCorporateMessage && Equals((RabbitMqCorporateMessage)obj);
        }

        public bool Equals(RabbitMqCorporateMessage other)
        {
            return other.DeliveryTag == DeliveryTag && Equals(other._bytes, _bytes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DeliveryTag.GetHashCode() * 397) ^ (_bytes != null ? _bytes.GetHashCode() : 0);
            }
        }

        public static bool operator ==(RabbitMqCorporateMessage one, RabbitMqCorporateMessage another)
        {
            return one.Equals(another);
        }

        public static bool operator !=(RabbitMqCorporateMessage one, RabbitMqCorporateMessage another)
        {
            return !(one == another);
        }

        #endregion
    }

}