using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq
{
    public sealed class RabbitMqQueueFactory : IRabbitMqQueueFactory
    {
        private readonly ConnectionFactory _connectionFactory;
        private bool _clientCanValidateQueueExist;

        public RabbitMqQueueFactory(string connectionString)
        {
            _connectionFactory = new ConnectionFactory();
            RabbitMqQueueFactoryConfigurator.Configure(_connectionFactory, connectionString);
        }

        private void DetectServerRestrictions()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var serverVersionString = Encoding.ASCII.GetString((byte[])connection.ServerProperties["version"]);

                if (string.IsNullOrEmpty(serverVersionString))
                    return;

                Version serverVersion;
                if (!Version.TryParse(serverVersionString, out serverVersion))
                    return;

                // only since RabbitMq 2.6 we can validate existing of a queue
                if (serverVersion >= new Version(2, 6))
                    _clientCanValidateQueueExist = true;
            }
        }

        #region ICorporateQueueFactory implementation

        public IRabbitMqQueueReader CreateQueueReader(string name)
        {
            DetectServerRestrictions();

            var connection = _connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            // ensure queue exists
            if (_clientCanValidateQueueExist)
                try
                {
                    model.QueueDeclarePassive(name);
                }
                catch (OperationInterruptedException)
                {
                    connection.Close();
                    throw;
                }

            return new QueueAdapter(connection, model, name);
        }

        public IRabbitMqQueueWriter CreateQueueWriter(string name)
        {
            DetectServerRestrictions();

            var connection = _connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            // ensure exchange exists
            if (_clientCanValidateQueueExist)
                try
                {
                    model.ExchangeDeclarePassive(name);
                }
                catch (OperationInterruptedException)
                {
                    connection.Close();
                    throw;
                }

            return new ExchangeAdapter(connection, model, name);
        }

        #endregion

        #region nested types

        private static class RabbitMqQueueFactoryConfigurator
        {
            public static void Configure(ConnectionFactory connectionFactory, string rabbitMqConnectionString)
            {
                var dbConnectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = rabbitMqConnectionString };

                var amqpTcpEndpoint = new AmqpTcpEndpoint
                {
                    Protocol = Protocols.AMQP_0_9_1,
                    Ssl = new SslOption(),
                };

                foreach (KeyValuePair<string, object> setting in dbConnectionStringBuilder)
                {
                    var settingValue = (string)setting.Value;

                    if (setting.Key.IndexOf("Hostname", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        amqpTcpEndpoint.HostName = settingValue;
                        continue;
                    }
                    if (setting.Key.IndexOf("Port", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        amqpTcpEndpoint.Port = int.Parse(settingValue, NumberStyles.None, CultureInfo.InvariantCulture);
                        continue;
                    }
                    if (setting.Key.IndexOf("VirtualHost", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        connectionFactory.VirtualHost = settingValue;
                        continue;
                    }
                    if (setting.Key.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        connectionFactory.UserName = settingValue;
                        continue;
                    }
                    if (setting.Key.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        connectionFactory.Password = settingValue;
                    }
                }

                connectionFactory.Endpoint = amqpTcpEndpoint;
            }
        }

        private sealed class QueueAdapter : IRabbitMqQueueReader
        {
            private readonly IConnection _connection;
            private readonly IModel _model;

            private readonly string _queueName;
            public string Name { get { return _queueName; } }

            private QueueEnumerator _readAndRemoveEnumerator;

            public QueueAdapter(IConnection connection, IModel model, string queueName)
            {
                _connection = connection;
                _model = model;
                _queueName = queueName;
            }

            public IEnumerable<RabbitMqCorporateMessage> ReadAndRemove()
            {
                return _readAndRemoveEnumerator ?? (_readAndRemoveEnumerator = new QueueEnumerator(_model, _queueName, true));
            }

            #region IEnumerable implementation

            public IEnumerator<RabbitMqCorporateMessage> GetEnumerator()
            {
                return new QueueEnumerator(_connection.CreateModel(), _queueName, false);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Dispose()
            {
                _connection.Close();
            }

            #endregion

            #region nested types

            private sealed class QueueEnumerator : IEnumerable<RabbitMqCorporateMessage>, IEnumerator<RabbitMqCorporateMessage>
            {
                private readonly IModel _model;
                private readonly string _queueName;
                private readonly bool _readAndRemove;

                public RabbitMqCorporateMessage Current { get; private set; }
                object IEnumerator.Current { get { return Current; } }

                public QueueEnumerator(IModel model, string queueName, bool readAndRemove)
                {
                    _model = model;
                    _queueName = queueName;
                    _readAndRemove = readAndRemove;
                }

                public bool MoveNext()
                {
                    var result = _model.BasicGet(_queueName, _readAndRemove);
                    if (result == null)
                        return false;

                    Current = new RabbitMqCorporateMessage(result.DeliveryTag, result.Body);
                    return true;
                }

                public void Dispose()
                {
                    if (!_readAndRemove)
                        _model.Dispose();
                }

                public void Reset() { }

                public IEnumerator<RabbitMqCorporateMessage> GetEnumerator() { return this; }
                IEnumerator IEnumerable.GetEnumerator() { return this; }
            }

            #endregion
        }

        private sealed class ExchangeAdapter : IRabbitMqQueueWriter
        {
            private readonly IConnection _connection;
            private readonly IModel _model;

            private readonly string _exchangeName;
            public string Name { get { return _exchangeName; } }

            public ExchangeAdapter(IConnection connection, IModel model, string exchangeName)
            {
                _connection = connection;
                _model = model;
                _exchangeName = exchangeName;
            }

            public void Write(byte[] message)
            {
                Write(_exchangeName, message);
            }

            public void Write(string key, byte[] message)
            {
                _model.BasicPublish(_exchangeName, key, false, false, null, message);
            }

            public void Dispose()
            {
                _connection.Close();
            }
        }

        #endregion
    }
}